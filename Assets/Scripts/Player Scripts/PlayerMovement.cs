using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region serializedFields
    [Header("Speed")]
    [SerializeField, Range(0f, 100f)]
    float WalkSpeed = 10f, RunSpeed = 15f, maxClimbSpeed = 2f, maxSwimSpeed = 5f, slowDownSpeed, CrouchSpeed = 6, maxJumpSpeed = 15;
    public bool onBelt;
    [Header("Acceleration"), Space(2)]
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f, maxAirAcceleration = 1f, maxClimbAcceleration = 40f, maxSwimAcceleration = 5f;
    [Space(5)]
    [SerializeField, Range(0f, 10f)]
    public float jumpHeight = 2f;
    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;
    [SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f, maxStairsAngle = 50f;
    [SerializeField, Range(90, 180)]
    float maxClimbAngle = 140f;
    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;
    [SerializeField, Min(0f)]
    float probeDistance = 1f;
    [SerializeField, Min(1.1f)]
    float wallJumpSpeed;
    [SerializeField, Range(1.5f, 5f)]
    float WallJumpIntensifire = 2f;
    [Header("Layers"), Space(2)]
    [SerializeField]
    LayerMask probeMask = -1, stairsMask = -1, climbMask = -1, waterMask = 0;

    [Header("Camera"), Space(2)]
    [SerializeField]
    Transform playerInputSpace = default;

    [SerializeField, Range(0f, .5f)]
    float turnSmoothTime = .1f;

    [Header("For Water"), Space(2)]
    [SerializeField]
    float submergenceOffset = 0.5f;
    [SerializeField, Min(0.1f)]
    float submergenceRange = 1f;
    [SerializeField, Range(0f, 10f)]
    float waterDrag = 1f;
    [SerializeField, Min(0f)]
    float buoyancy = 1f;
    [SerializeField, Range(0.01f, 1f)]
    float swimThreshold = 0.5f;
    [Header("Animator"), Space(5)]
    [SerializeField]
    Animator anim;
    [HideInInspector]
    public bool CanWallJump;
    [HideInInspector]
    public bool CantMove;

    #endregion
    #region private fields
    float CurrentSpeed;
    bool Isrunning;
    Vector3 playerInput;
    Vector3 connectionVelocity;
    Rigidbody body, connectedBody, previousConnectedBody;
    bool desiredJump, desiresClimbing;
    public bool OnGround => groundContactCount > 0;
    public bool onBlock;
    bool Swimming => submergence >= swimThreshold;
    float submergence;
    bool InWater => submergence > 0f;
    //[HideInInspector]
    public int jumpPhase;
    [HideInInspector]
    public bool canJump = true;

    [HideInInspector]
    public bool Bounce = false;

    float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;
    Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal;
    int groundContactCount, steepContactCount, climbContactCount;
    bool Climbing => climbContactCount > 0 && stepsSinceLastJump > 2;
    int stepsSinceLastGrounded, stepsSinceLastJump;
    bool OnSteep => steepContactCount > 0;
    Vector3 upAxis, rightAxis, forwardAxis;
    Vector3 connectionWorldPosition, connectionLocalPosition;
    float turnSmoothVelocity;
    //this will be for judging if you should play fall animation
    private float FallTimer = 2;
    private bool HoldingSpace;
    //for crouching 
    private Vector3 ColliderScale;
    private Vector3 ColliderCenter;
    private Vector3 velocity;
    [Header("Offsets")]
    [SerializeField, Range(0, .5f)]
    float CrouchOffsetY = .1f;
    bool isCrouching;
    public GameObject LastWallJumpedOn;

    #endregion
    #region MonoBehaviors
    void OnValidate()
    {
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        minGroundDotProduct = Mathf.Cos(maxGroundAngle);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerInputSpace = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (GetComponent<BoxCollider>() != null)
        {
            ColliderScale = GetComponent<BoxCollider>().size;
            ColliderCenter = GetComponent<BoxCollider>().center;
        }
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        OnValidate();
    }
    void Update()
    {

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Death") && !CantMove)
        {
            if(OnGround)
            {
                FallTimer = 2;
            }
            else
            {
                FallTimer -= Time.deltaTime;
            }
            if (OnGround && !Bounce)
            {
                canJump = true;
                CanWallJump = true;
                if(jumpPhase > 0 && !desiredJump)
                {
                    PlayAnimation("IsLanded");
                }
                StopAnimation("Falling");
                if (!desiredJump && velocity.y < 0.1f)
                {
                    StopAnimation("Jump");
                    StopAnimation("DoubleJump");
                }
                LastWallJumpedOn = null;
            }
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("idle") && !OnGround && !Bounce && FallTimer < 0)
            {
                PlayFallingAnimation();
            }
            if(!OnGround || jumpPhase == 0)
            {
                StopAnimation("IsLanded");
            }
            minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);

            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            playerInput.z = Swimming ? Input.GetAxis("UpDown") : 0f;
            playerInput = Vector3.ClampMagnitude(playerInput, 1f);
            if (!isCrouching || InWater || Climbing || !OnGround)
            {
                StopAnimation("Crouching");
                isCrouching = false;
            }
            if (isCrouching && !InWater && !Climbing && OnGround)
            {
                PlayAnimation("Crouching");
                CurrentSpeed = CrouchSpeed;
                isCrouching = true;
            }
            if (playerInputSpace)
            {
                rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
                forwardAxis = ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
            }
            else
            {
                rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
                forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
            }
            if (Swimming)
            {
                desiresClimbing = false;
            }
            else
            {
                isCrouching = Input.GetButton("Crouch");
                desiredJump |= Input.GetButtonDown("Jump");
                desiresClimbing = Input.GetButton("Climb");
                Isrunning = Input.GetButton("Run");

            }
            if (Isrunning && !isCrouching && !InWater && !Climbing && !CheckSteepContacts())
            {
                CurrentSpeed = RunSpeed;
            }
            if (!Isrunning && !isCrouching || InWater || Climbing || CheckSteepContacts())
            {
                CurrentSpeed = WalkSpeed;
            }
            if (isCrouching)
            {
                if (GetComponent<BoxCollider>() != null)
                {
                    GetComponent<BoxCollider>().size =
                    new Vector3(ColliderScale.x, ColliderScale.y / 1.2f, ColliderScale.z);
                    GetComponent<BoxCollider>().center = ColliderCenter - new Vector3(0, CrouchOffsetY, 0);
                }
            }
            else
            {
                if (GetComponent<BoxCollider>() != null)
                {
                    GetComponent<BoxCollider>().size = ColliderScale;
                    GetComponent<BoxCollider>().center = ColliderCenter;
                }
            }
            if (playerInput.magnitude < 0.6f && !onBelt)
            {
                body.velocity = new Vector3(0, body.velocity.y, 0);
            }
        }
        else
        {
            if (!CantMove)
                body.velocity = Vector3.zero;
            else
                body.velocity = new Vector3(0, body.velocity.y, 0);
        }

    }

    void FixedUpdate()
    {

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Death") && !CantMove)
        {
            Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
            UpdateState();

            if (InWater)
            {
                velocity *= 1f - waterDrag * submergence * Time.deltaTime;
            }
            AdjustVelocity();

            if (desiredJump)
            {

                PlayAnimation("Jump");
                Jump(gravity);
                desiredJump = false;
            }
            if (Climbing)
            {
                velocity -= contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
            }
            else if (OnGround && velocity.sqrMagnitude < 0.01f)
            {
                velocity += contactNormal * (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
            }
            else if (InWater)
            {
                if (!Input.GetKey(KeyCode.Space))
                {
                    velocity -= gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    velocity += gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
                }
            }
            else if (desiresClimbing && OnGround)
            {
                velocity += (gravity - contactNormal * (maxClimbAcceleration * 0.9f)) * Time.deltaTime;
            }
            else
            {
                velocity += gravity * Time.deltaTime;
            }
            if(OnGround)
                body.gameObject.GetComponent<PlayerMovement>().Bounce = false;
            body.velocity = velocity;
        }
        ClearState();
    }
    #endregion
    void ClearState()
    {
        groundContactCount = steepContactCount = climbContactCount = 0;
        contactNormal = steepNormal = climbNormal = Vector3.zero;
        connectionVelocity = Vector3.zero;
        previousConnectedBody = connectedBody;
        connectedBody = null;
        submergence = 0f;
    }
    bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2)
        {
            return false;
        }
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed)
        {
            return false;
        }
        if (!Physics.Raycast(body.position, -upAxis, out RaycastHit hit, probeDistance, probeMask,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }
        if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer))
        {
            return false;
        }
        float upDot = Vector3.Dot(upAxis, hit.normal);
        if (upDot < GetMinDot(hit.collider.gameObject.layer))
        {
            return false;
        }
        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f)
        {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        connectedBody = hit.rigidbody;
        return true;
    }
    float GetMinDot(int layer)
    {
        return (stairsMask & (1 << layer)) == 0 ?
            minGroundDotProduct : minStairsDotProduct;
    }
    void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        velocity = body.velocity;
        if (CheckClimbing() || CheckSwimming() || OnGround || SnapToGround() || CheckSteepContacts())
        {
            stepsSinceLastGrounded = 0;
            if (stepsSinceLastJump > 1)
            {
                jumpPhase = 0;
            }
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = upAxis;
        }
        if (connectedBody)
        {
            if (connectedBody.isKinematic || connectedBody.mass >= body.mass)
            {
                UpdateConnectionState();
            }
        }
    }
    void UpdateConnectionState()
    {
        if (connectedBody == previousConnectedBody)
        {
            Vector3 connectionMovement = connectedBody.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }
        connectionWorldPosition = body.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(connectionWorldPosition);
    }
    #region Collision && triggers
    void OnTriggerEnter(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0)
        {
            EvaluateSubmergence(other);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0)
        {
            EvaluateSubmergence(other);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
        //if (LastWallJumpedOn == collision.gameObject)
        //    return;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (!OnGround && contact.normal.y < 0.1f && LastWallJumpedOn != collision.gameObject && Input.GetKey(KeyCode.Space) && collision.gameObject.layer != 9 && CanWallJump)
            {
                Vector3 _velocity = contact.normal;

                _velocity.y = jumpHeight * WallJumpIntensifire;

                body.velocity = new Vector3(_velocity.x * (RunSpeed * wallJumpSpeed),
                    _velocity.y, _velocity.z * (RunSpeed * wallJumpSpeed));

                PreventSnapToGround();
                LastWallJumpedOn = collision.gameObject;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
        if (collision.gameObject.tag == "BuilderBlock")
            onBlock = true;
        else
            onBlock = false;
    }
    #endregion
    bool CheckSteepContacts()
    {
        if (steepContactCount > 1)
        {
            steepNormal.Normalize();
            float upDot = Vector3.Dot(upAxis, steepNormal);
            if (upDot >= minGroundDotProduct)
            {
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }
    void EvaluateSubmergence(Collider collider)
    {
        if (Physics.Raycast(body.position + upAxis * submergenceOffset, -upAxis,
            out RaycastHit hit, submergenceRange + 1f, waterMask, QueryTriggerInteraction.Collide))
        {
            submergence = 1f - hit.distance / submergenceRange;
        }
        else
        {
            submergence = 1f;
        }
        if (Swimming)
        {
            connectedBody = collider.attachedRigidbody;
        }
    }

    void EvaluateCollision(Collision collision)
    {
        if (Swimming)
        {
            return;
        }
        int layer = collision.gameObject.layer;
        float minDot = GetMinDot(layer);
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.gameObject.tag == "Grate" && GetComponent<BlobBert>() || collision.gameObject.tag == "CantJump")
            {
                return;
            }
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(upAxis, normal);
            if (upDot >= minDot)
            {
                groundContactCount += 1;
                contactNormal += normal;
                connectedBody = collision.rigidbody;
            }
            else
            {
                if (upDot > -0.01f)
                {
                    steepContactCount += 1;
                    steepNormal += normal;
                    if (groundContactCount == 0)
                    {
                        connectedBody = collision.rigidbody;
                    }
                }
                if (desiresClimbing && upDot >= minClimbDotProduct && (climbMask & (1 << layer)) != 0)
                {
                    climbContactCount += 1;
                    climbNormal += normal;
                    lastClimbNormal = normal;
                    connectedBody = collision.rigidbody;
                }
            }

        }
    }

    Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
    {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }
    void AdjustVelocity()
    {
        float acceleration, speed;
        Vector3 xAxis, zAxis;
        if (Climbing)
        {
            acceleration = maxClimbAcceleration;
            speed = maxClimbSpeed;
            xAxis = Vector3.Cross(contactNormal, upAxis);
            zAxis = upAxis;
        }
        else if (InWater)
        {
            float swimFactor = Mathf.Min(1f, submergence / swimThreshold);

            acceleration = Mathf.LerpUnclamped(OnGround ? maxAcceleration : maxAirAcceleration, maxSwimAcceleration, swimFactor);
            speed = Mathf.LerpUnclamped(CurrentSpeed, maxSwimSpeed, swimFactor);

            xAxis = rightAxis;
            zAxis = forwardAxis;
        }
        else
        {
            acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
            //this right here is causing a big problem for holding space when walking
            speed = OnGround && desiresClimbing ? maxClimbSpeed : CurrentSpeed;
            xAxis = rightAxis;
            zAxis = forwardAxis;
        }
        //this is temp code 
        if (Isrunning && !isCrouching && !InWater && !Climbing && !CheckSteepContacts())
        {
            maxClimbSpeed = RunSpeed;
        }
        if (!Isrunning && !isCrouching || InWater || Climbing || CheckSteepContacts())
        {
            maxClimbSpeed = WalkSpeed;
        }
        xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

        Vector3 relativeVelocity = velocity - connectionVelocity;
        float currentX = Vector3.Dot(relativeVelocity, xAxis);
        float currentZ = Vector3.Dot(relativeVelocity, zAxis);

        float maxSpeedChange = acceleration * 20f * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, playerInput.x * speed, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, playerInput.y * speed, maxSpeedChange);

        //rotation
        if (!Climbing && !CheckSteepContacts() && playerInput.magnitude > .1f)
        {
            float targetAngle = Mathf.Atan2(newX, newZ) * Mathf.Rad2Deg + playerInputSpace.localEulerAngles.y;

            //used to smooth the angle needed to move to avoid snapping to directions
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //rotate player
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }
        if (CheckSteepContacts())
        {
            //rotate player
            transform.rotation = Quaternion.Euler(0f, velocity.y, 0f);
        }
        //------------------------
        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);

        if (velocity.magnitude > 1f && !InWater)
        {
            //if (velocity.y < .1f)
            //{
                PlayAnimation("Walk");
                if (Isrunning)
                {
                    PlayAnimation("Run");
                }
                else
                {
                    StopAnimation("Run");
                }
            //}
        }
        if (InWater || velocity.magnitude < 1f)
        {
            StopAnimation("Run");
            StopAnimation("Walk");
        }
        if (Swimming)
        {

            float currentY = Vector3.Dot(relativeVelocity, upAxis);
            float newY = Mathf.MoveTowards(currentY, playerInput.z * speed, maxSpeedChange);
            velocity += upAxis * (newY - currentY);
        }
    }
    void Jump(Vector3 gravity)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("DoubleJump") || jumpPhase > maxAirJumps)
        {
            canJump = false;
        }
        if (canJump)
        {
            Vector3 jumpDirection;
            if (OnGround)
            {
                jumpDirection = contactNormal;
            }
            else if (OnSteep)
            {
                jumpDirection = steepNormal;
                jumpPhase = 1;
            }
            else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps)
            {
                if (jumpPhase == 0)
                {
                    jumpPhase = 1;
                }
                jumpDirection = contactNormal;
            }
            else
            {
                return;
            }
            if (jumpPhase == 1)
            {
                PlayAnimation("DoubleJump");
            }

            stepsSinceLastJump = 0;
            jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);
            if (InWater)
            {
                jumpSpeed *= Mathf.Max(0f, 1f - submergence / swimThreshold);
            }
            jumpDirection = (jumpDirection + upAxis).normalized;
            float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
            if (alignedSpeed > 0f)
            {
                if (jumpPhase == 0)
                    jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
                else
                    jumpSpeed = Mathf.Max(jumpSpeed / 2 - alignedSpeed, 0f);
            }
            if (jumpSpeed < 10)
            {
                jumpDirection.z = 0;
                jumpSpeed = 16;
            }
            velocity += jumpDirection * jumpSpeed;
            velocity.y = Mathf.Clamp(velocity.y, -20, maxJumpSpeed);
        }
    }
    bool CheckClimbing()
    {
        if (Climbing)
        {
            if (climbContactCount > 1)
            {
                climbNormal.Normalize();
                float upDot = Vector3.Dot(upAxis, climbNormal);
                if (upDot >= minGroundDotProduct)
                {
                    climbNormal = lastClimbNormal;
                }
            }
            groundContactCount = 1;
            contactNormal = climbNormal;
            return true;
        }
        return false;
    }
    bool CheckSwimming()
    {
        if (Swimming)
        {
            groundContactCount = 0;
            contactNormal = upAxis;
            return true;
        }
        return false;
    }
    public void PreventSnapToGround()
    {
        stepsSinceLastJump = -1;
    }
    #region Bounce Functions
    public void jumpOnEnemy()
    {
        body.velocity = new Vector3(body.velocity.x, body.velocity.y + 4, body.velocity.z);
        jumpPhase = 0;
    }
    public void jumpOnBouncePad(float distance)
    {
        body.velocity = new Vector3(body.velocity.x, jumpHeight + distance, body.velocity.z);
        jumpPhase = 0;
    }
    
    public void GroundPound()
    {
        body.velocity = new Vector3(0, -20, 0);
        //this is here to ensure you wont double jump
        jumpPhase = 20;
    }
    #endregion
    #region Animation
    /// <summary>
    /// Call this for anytime you need to play an animation 
    /// </summary>
    /// <param name="animName"></param>
    public void PlayAnimation(string BoolName)
    {
        anim.SetBool(BoolName, true);
    }
    /// <summary>
    /// Call this for anytime you need to stop an animation
    /// </summary>
    /// <param name="BoolName"></param>
    public void StopAnimation(string BoolName)
    {
        anim.SetBool(BoolName, false);
    }
    public void PlayFallingAnimation()
    {
        PlayAnimation("Falling");
    }
    public void PlayJumpAnimation()
    {
        PlayAnimation("Jump");
    }
    #endregion
}
