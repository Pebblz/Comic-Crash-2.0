using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Photon.Realtime;
using Photon.Pun;
public class PlayerMovement : MonoBehaviour
{
    #region serializedFields
    [Header("Speed / Pat varibles")]
    [SerializeField, Range(0f, 100f)]
    float WalkSpeed = 10f, RunSpeed = 15f, maxClimbSpeed = 2f, maxSwimSpeed = 5f, slowDownSpeed, CrouchSpeed = 6, maxJumpSpeed = 15, WalkUnderWaterSpeed = 5f, RunUnderWaterSpeed = 8f;

    [SerializeField, Range(1f, 20f), Tooltip("How fast you swim up when holding jump button")]
    float SwimUpSpeed = 4f;

    [SerializeField, Range(1f, 20f), Tooltip("How fast you float down when holding no button")]
    float SlowlyFloatDownSpeed = 4f;

    [SerializeField, Range(1f, 20f), Tooltip("How fast you go down when Crouching")]
    float SwimmingDownSpeed = 4f;
    [SerializeField, Range(.1f, 1), Tooltip("The speed of underwater Animations")]
    float WalkAnimationSpeed = .5f, RunAnimationSpeed = .5f;
    [SerializeField, Range(10f, 50f), Tooltip("How fast you shoot forward when diving")]
    float AirDiveSpeed = 10f;

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
    [SerializeField, Range(0f, 20f)]
    float waterDrag = 1f;
    [SerializeField, Min(0f)]
    float buoyancy = 1f;
    [SerializeField, Range(0.01f, 1f)]
    float swimThreshold = 0.5f;
    [SerializeField, Range(.1f, 5f)]
    float AtTopOfWaterOffset = 1;

    [Header("Animator"), Space(5)]
    [SerializeField]
    Animator anim;


    [Header("Offsets")]
    [SerializeField, Range(0, .5f)]
    float CrouchOffsetY = .1f;

    [HideInInspector]
    public bool onBelt;
    [HideInInspector]
    public bool CanWallJump;
    [HideInInspector]
    public bool CantMove;
    [HideInInspector]
    public GameObject LastWallJumpedOn;
    [HideInInspector]
    public bool onBlock;
    [HideInInspector]
    public int jumpPhase;
    [HideInInspector]
    public bool canJump = true;
    [HideInInspector]
    public bool Bounce = false;
    public bool CanDive = false;
    [HideInInspector]
    public bool inWaterAndFloor;
    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public bool AtTheTopOfWater;

    public bool CollectibleGotten;

    [SerializeField, Tooltip("The length of time for collecting a collectible")]
    float collectibleTimer;
    float currentCollectibleTimer;

    public bool InWater => submergence > 0f;
    #endregion
    #region private fields
    float CurrentSpeed;
    bool Isrunning;
    Vector3 playerInput;
    Vector3 connectionVelocity;
    Rigidbody body, connectedBody, previousConnectedBody;
    bool desiredJump, desiresClimbing;
    public bool OnGround => groundContactCount > 0;

    public bool Swimming => submergence >= swimThreshold;
    float submergence;


    Collider water;
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
    private bool OnFloor;

    bool isCrouching;
    bool blobert, handman;

    PhotonView photonView;

    [Header("Particles")]
    [SerializeField] ParticleSystem walkingPartical1;
    [SerializeField] ParticleSystem walkingPartical2;
    [SerializeField] ParticleSystem walkingPartical3;
    [SerializeField] ParticleSystem walkingPartical4;
    #endregion
    #region MonoBehaviors
    void OnValidate()
    {
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        minGroundDotProduct = Mathf.Cos(maxGroundAngle);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        anim = GetComponent<Animator>();
        playerInputSpace = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (GetComponent<BoxCollider>() != null)
        {
            ColliderScale = GetComponent<BoxCollider>().size;
            ColliderCenter = GetComponent<BoxCollider>().center;
        }
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        if (GetComponent<BlobBert>())
        {
            blobert = true;
        }
        if (GetComponent<HandMan>())
        {
            handman = true;
        }
        OnValidate();
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Death") && !CantMove)
            {
                if (OnGround && !inWaterAndFloor)
                {
                    FallTimer = 2;
                    if (walkingPartical1 != null && walkingPartical1.isStopped)
                        walkingPartical1.Play();
                    if (walkingPartical2 != null && walkingPartical2.isStopped)
                        walkingPartical2.Play();
                    if (walkingPartical3 != null && walkingPartical3.isStopped)
                        walkingPartical3.Play();
                    if (walkingPartical4 != null && walkingPartical4.isStopped)
                        walkingPartical4.Play();
                }
                else
                {
                    FallTimer -= Time.deltaTime;
                    if (walkingPartical1 != null && walkingPartical1.isPlaying)
                        walkingPartical1.Stop();
                    if (walkingPartical2 != null && walkingPartical2.isPlaying)
                        walkingPartical2.Stop();
                    if (walkingPartical3 != null && walkingPartical3.isPlaying)
                        walkingPartical3.Stop();
                    if (walkingPartical4 != null && walkingPartical4.isPlaying)
                        walkingPartical4.Stop();
                }
                if (OnGround && !Bounce)
                {
                    canJump = true;
                    CanWallJump = true;
                    if (CanDive)
                        StopAnimation("Dive");

                    if (jumpPhase > 0 && !desiredJump)
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
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("idle") && !OnGround && !Bounce && FallTimer < 0 && !Swimming && !InWater)
                {
                    PlayFallingAnimation();
                }
                if (!OnGround || jumpPhase == 0)
                {
                    StopAnimation("IsLanded");
                }
                minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);

                playerInput.x = InputManager.GetAxis("Horizontal");
                playerInput.y = InputManager.GetAxis("Vertical");
                playerInput.z = Swimming ? InputManager.GetAxis("UpDown") : 0f;
                playerInput = Vector3.ClampMagnitude(playerInput, 1f);
                if (!isCrouching || InWater || Climbing || !OnGround || inWaterAndFloor)
                {
                    StopAnimation("Crouching");
                    isCrouching = false;
                }
                if (isCrouching && !InWater && !Climbing && OnGround && !inWaterAndFloor)
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
                    if (!InWater)
                    {
                        StopAnimation("Sinking");
                        StopAnimation("SwimmingDown");
                        StopAnimation("SwimmingUp");
                        StopAnimation("SwimLeftOrRight");
                        if (blobert)
                            StopAnimation("Drowning");
                    }
                    if (OnGround)
                        isCrouching = InputManager.GetButton("Crouch");

                    desiredJump |= InputManager.GetButtonDown("Jump");
                    desiresClimbing = InputManager.GetButton("Climb");
                    Isrunning = InputManager.GetButton("Sprint");

                }
                if (!inWaterAndFloor)
                {
                    if (Isrunning && !isCrouching && !InWater && !Climbing && !CheckSteepContacts())
                    {
                        CurrentSpeed = RunSpeed;
                        SetAnimatorFloat("RunMultiplier", 1);
                    }
                    if (!Isrunning && !isCrouching || InWater || Climbing || CheckSteepContacts())
                    {
                        CurrentSpeed = WalkSpeed;
                        SetAnimatorFloat("WalkMultiplier", 1);
                    }
                }
                else
                {
                    if (Isrunning && !isCrouching && !CheckSteepContacts())
                    {
                        CurrentSpeed = RunUnderWaterSpeed;
                        SetAnimatorFloat("RunMultiplier", RunAnimationSpeed);
                    }
                    if (!Isrunning && !isCrouching || CheckSteepContacts())
                    {
                        CurrentSpeed = WalkUnderWaterSpeed;
                        SetAnimatorFloat("WalkMultiplier", WalkAnimationSpeed);
                    }
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
            if (CollectibleGotten && currentCollectibleTimer <= 0 && !CantMove)
            {
                currentCollectibleTimer = collectibleTimer;
            }
            if (CollectibleGotten && currentCollectibleTimer <= 0 && CantMove)
            {
                DoneWithCollectible();
                CollectibleGotten = false;
            }
            if (currentCollectibleTimer > 0)
            {
                GotCollectible();
            }
            currentCollectibleTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
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
                    jumpPhase = 0;
                    StopAnimation("Falling");
                    if (!blobert)
                    {
                        //If your not holding space or crouch
                        if (playerInput.z == 0 && playerInput.x == 0 && playerInput.y == 0)
                        {
                            PlayAnimation("Sinking");
                            StopAnimation("SwimmingDown");
                            StopAnimation("SwimmingUp");
                            StopAnimation("SwimLeftOrRight");
                            //velocity -= gravity * ((-SlowlyFloatDownSpeed - buoyancy * submergence) * Time.deltaTime);
                        }
                        //If your holding crouch
                        if (playerInput.z < 0)
                        {
                            PlayAnimation("SwimmingDown");
                            StopAnimation("SwimmingUp");
                            StopAnimation("Sinking");
                            StopAnimation("SwimLeftOrRight");
                            velocity -= gravity * ((-SwimmingDownSpeed - buoyancy * submergence) * Time.deltaTime);
                        }
                        //If your holding space 
                        if (playerInput.z > 0 && (water.GetComponent<Transform>().GetChild(0).transform.position.y) > transform.position.y)
                        {
                            PlayAnimation("SwimmingUp");
                            StopAnimation("Sinking");
                            StopAnimation("SwimmingDown");
                            StopAnimation("SwimLeftOrRight");
                            velocity -= gravity * ((SwimUpSpeed - buoyancy * submergence) * Time.deltaTime);
                        }
                        //If your moving left or right
                        if (playerInput.x != 0 && playerInput.z == 0 || playerInput.y != 0 && playerInput.z == 0)
                        {
                            PlayAnimation("SwimLeftOrRight");
                            StopAnimation("Sinking");
                            StopAnimation("SwimmingDown");
                            StopAnimation("SwimmingUp");
                        }
                    }
                    else
                    {
                        PlayAnimation("Drowning");
                        if (Swimming)
                            velocity -= gravity * ((SwimUpSpeed - buoyancy * submergence) * Time.deltaTime);
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
                if (!OnGround && inWaterAndFloor)
                {
                    EvaluateSubmergence(water);
                }

                if (water != null)
                {
                    if ((water.GetComponent<Transform>().GetChild(0).transform.position.y) - 1 < transform.position.y)
                    {
                        AtTheTopOfWater = true;
                    }
                    else
                    {
                        AtTheTopOfWater = false;
                    }
                }

                if (OnGround)
                    body.gameObject.GetComponent<PlayerMovement>().Bounce = false;
                body.velocity = velocity;
            }
            ClearState();
        }
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
        if ((waterMask & (1 << other.gameObject.layer)) != 0 && !OnFloor)
        {
            EvaluateSubmergence(other);
            water = other;
        }
        if ((waterMask & (1 << other.gameObject.layer)) != 0 && OnFloor)
        {
            inWaterAndFloor = true;
            water = other;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0 && !OnFloor)
        {
            EvaluateSubmergence(other);
            water = other;
        }
        if ((waterMask & (1 << other.gameObject.layer)) != 0 && OnFloor)
        {
            inWaterAndFloor = true;
            water = other;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0)
        {
            inWaterAndFloor = false;
        }

    }
    void OnCollisionEnter(Collision collision)
    {
        if (!blobert)
        {
            if (collision.gameObject.tag == "Floor")
            {
                OnFloor = true;
            }
            if (collision.gameObject.tag == "WalkableUnderWater")
            {
                if (collision.gameObject.transform.GetChild(0).GetComponent<UnderWaterSurfaces>().walkingOn)
                {
                    OnFloor = true;
                }
            }
        }
        EvaluateCollision(collision);
        if (LastWallJumpedOn == collision.gameObject)
            return;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (!OnGround && contact.normal.y < 0.1f && LastWallJumpedOn != collision.gameObject &&
                InputManager.GetButton("Jump") && collision.gameObject.layer != 9 &&
                collision.gameObject.layer != 10 && CanWallJump)
            {
                Vector3 _velocity = contact.normal;

                _velocity.y = jumpHeight * WallJumpIntensifire;

                body.velocity = new Vector3(_velocity.x * (4 * wallJumpSpeed),
                    _velocity.y, _velocity.z * (4 * wallJumpSpeed));

                PreventSnapToGround();
                LastWallJumpedOn = collision.gameObject;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            OnFloor = false;
        }
        if (collision.gameObject.tag == "WalkableUnderWater")
        {
            OnFloor = false;
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
        inWaterAndFloor = false;
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
        if (!Climbing && !CheckSteepContacts() && playerInput.x != 0f || !Climbing && !CheckSteepContacts() && playerInput.y != 0f)
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

        if (velocity.x != 0 && !InWater ||
            velocity.z != 0 && !InWater)
        {
            PlayAnimation("Walk");
            if (Isrunning)
            {
                PlayAnimation("Run");
            }
            else
            {
                StopAnimation("Run");
            }
        }
        if (InWater || velocity.magnitude < 1f)
        {
            StopAnimation("Run");
            StopAnimation("Walk");
        }
        //if (Swimming)
        //{

        //    float currentY = Vector3.Dot(relativeVelocity, upAxis);
        //    float newY = Mathf.MoveTowards(currentY, playerInput.z * speed, maxSpeedChange);
        //    velocity += upAxis * (newY - currentY);
        //}
    }
    //public void transferOwnership(PhotonView view)
    //{
        //if (view.IsMine)
            //photonView.TransferOwnership(view.ViewID);
    //}
    public void GotCollectible()
    {
        CantMove = true;
        playerInputSpace.GetComponent<MainCamera>().collectibleCamera = true;
        //PlayAnimation("Collected");
        StopAnimation("Run");
        StopAnimation("Walk");
        transform.LookAt(new Vector3(playerInputSpace.position.x, transform.position.y, playerInputSpace.position.z));
    }
    public void DoneWithCollectible()
    {
        CantMove = false;
        playerInputSpace.GetComponent<MainCamera>().collectibleCamera = false;
        //StopAnimation("Collected");
    }
    void Jump(Vector3 gravity)
    {
        if (!Swimming || !InWater)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("DoubleJump") || jumpPhase > maxAirJumps)
            {
                canJump = false;
            }
            if (jumpPhase == maxAirJumps + 1 && CanDive && body.velocity.x != 0f ||
                jumpPhase == maxAirJumps + 1 && CanDive && body.velocity.z != 0f)
            {
                PlayAnimation("Dive");
                Vector3 DiveDir = transform.forward * AirDiveSpeed;
                velocity = new Vector3(DiveDir.x, 0, DiveDir.z);
                jumpPhase = 5;
            }

            if (canJump)
            {
                StopAnimation("Dive");
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
                if (jumpPhase == 1 && !handman)
                {
                    PlayAnimation("DoubleJump");
                }
                if (jumpPhase == 1 && handman)
                {
                    if (GetComponent<HandMan>().isHoldingOBJ)
                    {
                        return;
                    }
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
    public void SetAnimatorFloat(string FloatName, float speed)
    {
        anim.SetFloat(FloatName, speed);
    }
    public void PlayFallingAnimation()
    {
        anim = GetComponent<Animator>();
        PlayAnimation("Falling");
    }
    public void PlayJumpAnimation()
    {
        anim = GetComponent<Animator>();
        PlayAnimation("Jump");
    }
    #endregion
}
