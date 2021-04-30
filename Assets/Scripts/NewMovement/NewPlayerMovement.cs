using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerMovement : MonoBehaviour
{
    #region serializedFields
    [Header("Speed")]
    [SerializeField, Range(0f, 100f)]
    float WalkSpeed = 10f, RunSpeed = 15f, maxClimbSpeed = 2f, maxSwimSpeed = 5f, slowDownSpeed;
    [Space(5)]
    [Header("Acceleration")]
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f, maxAirAcceleration = 1f, maxClimbAcceleration = 40f, maxSwimAcceleration = 5f;
    [Space(5)]
    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;
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
    [Space(5)]
    [Header("Layers")]
    [SerializeField]
    LayerMask probeMask = -1, stairsMask = -1, climbMask = -1, waterMask = 0;
    [Space(5)]
    [SerializeField]
    Transform playerInputSpace = default;
    [SerializeField, Range(0f, .5f)]
    float turnSmoothTime = .1f;
    [Space(5)]
    [Header("For Water")]
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
    [SerializeField]
    Animator anim;
    #endregion
    #region
    float CurrentSpeed;
    bool Isrunning;
    Vector3 playerInput;
    Vector3 velocity, connectionVelocity;
    Rigidbody body, connectedBody, previousConnectedBody;
    bool desiredJump, desiresClimbing;
    bool OnGround => groundContactCount > 0;
    bool Swimming => submergence >= swimThreshold;
    float submergence;
    bool InWater => submergence > 0f;
    int jumpPhase;
    float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;
    Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal;
    int groundContactCount, steepContactCount, climbContactCount;
    bool Climbing => climbContactCount > 0 && stepsSinceLastJump > 2;
    int stepsSinceLastGrounded, stepsSinceLastJump;
    bool OnSteep => steepContactCount > 0;
    Vector3 upAxis, rightAxis, forwardAxis;
    Vector3 connectionWorldPosition, connectionLocalPosition;
    float turnSmoothVelocity;
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
        body = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        body.useGravity = false;
        OnValidate();
    }
    void Update()
    { 
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);

        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput.z = Swimming ? Input.GetAxis("UpDown") : 0f;
        playerInput = Vector3.ClampMagnitude(playerInput, 1f);

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
            desiredJump |= Input.GetButtonDown("Jump");
            desiresClimbing = Input.GetButton("Climb");
            Isrunning = Input.GetButton("Run");
        }
        if (Isrunning)
        {
            CurrentSpeed = RunSpeed;
        }
        else
        {
            CurrentSpeed = WalkSpeed;
        }
        if (playerInput.magnitude < 0.3f)
        {
            body.velocity = new Vector3(0, body.velocity.y, 0);
        }
    }

    void FixedUpdate()
    {
        if (OnGround && !desiredJump && velocity.y < 0.1f)
        {
            StopAnimation("Jump");
            StopAnimation("DoubleJump");
        }
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
            velocity += gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
        }
        else if (desiresClimbing && OnGround)
        {
            velocity += (gravity - contactNormal * (maxClimbAcceleration * 0.9f)) * Time.deltaTime;
        }
        else
        {
            velocity += gravity * Time.deltaTime;
        }
        body.velocity = velocity;
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
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
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
            speed = OnGround && desiresClimbing ? maxClimbSpeed : CurrentSpeed;
            xAxis = rightAxis;
            zAxis = forwardAxis;
        }
        xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

        Vector3 relativeVelocity = velocity - connectionVelocity;
        float currentX = Vector3.Dot(relativeVelocity, xAxis);
        float currentZ = Vector3.Dot(relativeVelocity, zAxis);

        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, playerInput.x * speed, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, playerInput.y * speed, maxSpeedChange);

        //rotation
        if (!Climbing)
        {
            float targetAngle = Mathf.Atan2(newX, newZ) * Mathf.Rad2Deg + playerInputSpace.localEulerAngles.y;

            //used to smooth the angle needed to move to avoid snapping to directions
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //rotate player
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }
        //------------------------
        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);

        if (velocity.magnitude > 1f && !InWater && OnGround)
        {
            if (velocity.y < .01f)
            {
                if (Isrunning)
                {
                    PlayAnimation("Run");
                    StopAnimation("Walk");
                }
                else
                {
                    PlayAnimation("Walk");
                    StopAnimation("Run");
                }
            }
        }
        if (InWater || velocity.magnitude < 1f || !OnGround)
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
        Vector3 jumpDirection;
        if (OnGround)
        {
            jumpDirection = contactNormal;
        }
        else if (OnSteep)
        {
            jumpDirection = steepNormal;
            jumpPhase = 0;
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
        if(jumpPhase == 1)
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
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }
        velocity += jumpDirection * jumpSpeed;
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
    #endregion
}
