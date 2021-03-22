using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

    #region Speed Vars
    [Header("Movement speeds")]

    [SerializeField, Range(1f, 20f)] 
    private float WalkSpeed;

    [SerializeField, Range(.001f, 2f)] 
    private float SpeedAcceleration;

    [SerializeField, Range(.001f, 2f)]
    private float SpeedDeceleration;

    [SerializeField, Range(1f, 10f)]
    private int CrouchSpeed;

    [HideInInspector]
    public float currentSpeed;

    [SerializeField, Range(5f, 30f)]
    private float runSpeed;

    [SerializeField, Range(1f, 10f)]
    float jumpSpeed;

    [SerializeField, Range(.1f, .5f)]
    float turnSmoothTime = .1f;

    [HideInInspector]
    float curRollSpeed = 0;

    [SerializeField, Range(100f, 120f)]
    float maxRollSpeed;

    [SerializeField, Range(20f, 40f), Tooltip("How fast it'll Accelerate")]
    float RollAcceleration;

    [SerializeField, Range(.1f, 1f), Tooltip("How fast it'll Decelerate")]
    float RollDeceleration;

    [SerializeField, Range(0, 5), Tooltip("Amount of jumps allowed")]
    int jumpsAllowed;

    [SerializeField, Range(1f, 20f), Tooltip("The speed of the dash")]
    float DashSpeed;

    [SerializeField, Range(1f, 20f), Tooltip("The speed of the Slide")]
    float SlideSpeed;

    [SerializeField, Range(1f, 10f), Tooltip("The speed of the Grapple")]
    float GrappleSpeed;

    private int jumpsMade = 0;
    #endregion

    #region Other Vars
    private Rigidbody RB;

    float distToGround;

    private Vector3 MoveDir;

    Transform MainCam;

    float turnSmoothVelocity;


    [HideInInspector]
    public bool Roll;

    [HideInInspector]
    public bool LedgeGrabbing;

    private Animator anim;

    [SerializeField]
    float maxGroundAngle;
    [HideInInspector]
    public bool Grounded;

    private Vector3 ColliderScale;

    private Vector3 ColliderCenter;

    private float jumpTimer;
    [Header("Offsets")]

    [SerializeField, Range(0, .5f)]
    float CrouchOffsetY;

    [SerializeField, Range(.01f, 3f)]
    float LedgeOffset;

    [SerializeField, Range(.5f, 3f)]
    float LedgeGetUpOffset;

    [SerializeField, Range(1f, 90f)]
    float SlopeLimit;

    [HideInInspector]
    public bool IceFloor;

    [HideInInspector]
    public bool InGrapple;

    private bool DoneJumping;

    [HideInInspector]
    public GameObject Ledge;
    #endregion

    #region MonoBehaviours
    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y - .4f;
        RB = GetComponent<Rigidbody>();
        MainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        anim = GetComponent<Animator>();
        if (GetComponent<BoxCollider>() != null)
        {
            ColliderScale = GetComponent<BoxCollider>().size;
            ColliderCenter = GetComponent<BoxCollider>().center;
        }
    }

    void Update()
    {
        if (LedgeGrabbing)
        {
            Ledgegrabbing();
        }
    }
    void FixedUpdate()
    {
        float angle = GetGroundAngle();
        Grounded = IsGrounded();
        if (angle < maxGroundAngle)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                if (!InGrapple)
                {
                    if (!LedgeGrabbing)
                    {
                        StopAnimation("Hanging");
                        StopAnimation("Grapple");
                        RB.useGravity = true;
                        if (!Input.GetKey(KeyCode.C))
                        {
                            StopAnimation("Crouching");
                            if (!Roll)
                            {
                                regularMovement();
                            }
                            else
                            {
                                rolling();
                            }
                            Jump();
                        }
                        else if (Input.GetKey(KeyCode.C))
                        {
                            PlayAnimation("Crouching");
                            Crouch();
                            if (IsGrounded())
                            {
                                StopAnimation("Jump");
                                StopAnimation("DoubleJump");
                                StopAnimation("Dive");
                            }
                        }
                    }
                }
                else
                {
                    PlayAnimation("Grapple");
                    GrappleMovement();
                }
            }
        } else if(angle > maxGroundAngle || angle <= 0.01f)
        {

            RB.velocity = new Vector3(RB.velocity.x, -Mathf.Lerp(RB.velocity.y, 300, .05f), RB.velocity.z);
        }
    }
    #endregion

    #region Movement
    /// <summary>
    /// This function accounts for running walking and ice sliding 
    /// </summary>
    void regularMovement()
    {
        RB.constraints = RigidbodyConstraints.FreezeRotationX;
        RB.constraints = RigidbodyConstraints.FreezeRotationZ;

        float H = Input.GetAxisRaw("Horizontal");
        float V = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(H, 0, V).normalized;
        if (GetComponent<BoxCollider>() != null)
        {
            GetComponent<BoxCollider>().size = ColliderScale;
            GetComponent<BoxCollider>().center = ColliderCenter;
        }
        //running 
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (direction.magnitude >= 0.1f)
            {
                PlayAnimation("Run");
                if (currentSpeed < runSpeed)
                {
                    currentSpeed += SpeedAcceleration;
                }
            }
        }
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            if (direction.magnitude >= 0.1f)
            {
                StopAnimation("Run");
                if (currentSpeed < WalkSpeed)
                {
                    currentSpeed += SpeedAcceleration;
                }
                else if (currentSpeed > WalkSpeed)
                {
                    currentSpeed -= SpeedAcceleration;
                }
            }

        }
        if (direction.magnitude >= 0.1f)
        {
            PlayAnimation("Walk");

            //sees how much is needed to rotate to match camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + MainCam.localEulerAngles.y;

            //used to smooth the angle needed to move to avoid snapping to directions
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //rotate player
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            //converts rotation to direction / gives the direction you want to move in taking camera into account
            MoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        else
        {
            if (WalkSpeed < currentSpeed && anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                PlayAnimation("Skid");
            }
            else
            {
                StopAnimation("Walk");
                StopAnimation("Run");
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Skid"))
                {
                    StopAnimation("Skid");
                }
            }

        }
        //this is here just incase it gets set to a negative 
        if (currentSpeed < 0)
        {
            currentSpeed = 0;
        }
        if (currentSpeed > 0)
        {
            currentSpeed -= SpeedDeceleration;
        }
        if (!IceFloor && currentSpeed > .01f)
        {
            //this is here so when he walks he just flips backwords when he wants to move backwords
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                RB.MovePosition(transform.position += MoveDir.normalized * currentSpeed * Time.deltaTime);
            }
            //this is here so he can do a little loop when he goes from running forward to running backword
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                RB.MovePosition(transform.position += transform.forward * currentSpeed * Time.deltaTime);
            }
        }
        else if (IceFloor)
        {
            RB.AddForce(MoveDir.normalized * (currentSpeed += 5) * Time.deltaTime, ForceMode.VelocityChange);
        }


    }
    void GrappleMovement()
    {
        RB.constraints = RigidbodyConstraints.FreezeRotationX;
        RB.constraints = RigidbodyConstraints.FreezeRotationZ;

        float H = Input.GetAxisRaw("Horizontal");
        float V = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(H, 0, V).normalized;
        if (GetComponent<BoxCollider>() != null)
        {
            GetComponent<BoxCollider>().size = ColliderScale;
            GetComponent<BoxCollider>().center = ColliderCenter;
        }
        if (direction.magnitude >= 0.1f)
        {
            //sees how much is needed to rotate to match camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + MainCam.localEulerAngles.y;

            //used to smooth the angle needed to move to avoid snapping to directions
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //rotate player
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            //converts rotation to direction / gives the direction you want to move in taking camera into account
            MoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            RB.AddForce(MoveDir.normalized * GrappleSpeed * Time.deltaTime, ForceMode.VelocityChange);



        }
    }
    /// <summary>
    /// This is here for specifically Jeff
    /// </summary>
    void rolling()
    {
        RB.constraints = RigidbodyConstraints.None;

        float H = Input.GetAxisRaw("Horizontal");
        float V = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(H, 0, V).normalized;
        if (GetComponent<BoxCollider>() != null)
        {
            GetComponent<BoxCollider>().size = ColliderScale;
            GetComponent<BoxCollider>().center = ColliderCenter;
        }
        if (direction.magnitude >= 0.1f)
        {
            //this speeds up the player 
            if (curRollSpeed < maxRollSpeed)
            {
                curRollSpeed = curRollSpeed + (RollAcceleration * Time.deltaTime);
            }

            //sees how much is needed to rotate to match camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + MainCam.localEulerAngles.y;

            //used to smooth the angle needed to move to avoid snapping to directions
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //rotate player
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            //converts rotation to direction / gives the direction you want to move in taking camera into account
            MoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;



            //moves player via velocity
            RB.AddForce(MoveDir.normalized * curRollSpeed * Time.deltaTime);

        }
        else
        {
            if (curRollSpeed > 0)
            {
                //this slows the roll speed down so when you go back to rolling you don't start at max speed
                curRollSpeed = curRollSpeed - (RollAcceleration * Time.deltaTime);
            }
            //slows the player down 
            RB.velocity -= RollDeceleration * RB.velocity * Time.deltaTime;
        }

    }
    void Crouch()
    {

        RB.constraints = RigidbodyConstraints.FreezeRotationX;
        RB.constraints = RigidbodyConstraints.FreezeRotationZ;

        float H = Input.GetAxisRaw("Horizontal");
        float V = Input.GetAxisRaw("Vertical");

        if (GetComponent<BoxCollider>() != null)
        {
            GetComponent<BoxCollider>().size =
            new Vector3(ColliderScale.x, ColliderScale.y / 1.2f, ColliderScale.z);
            GetComponent<BoxCollider>().center = ColliderCenter - new Vector3(0, CrouchOffsetY, 0);
        }

        Vector3 direction = new Vector3(H, 0, V).normalized;

        if (direction.magnitude >= 0.1f)
        {
            PlayAnimation("Walk");
            //sees how much is needed to rotate to match camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + MainCam.localEulerAngles.y;

            //used to smooth the angle needed to move to avoid snapping to directions
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //rotate player
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            //converts rotation to direction / gives the direction you want to move in taking camera into account
            MoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            RB.MovePosition(transform.position += MoveDir.normalized * CrouchSpeed * Time.deltaTime);


        }
        else
        {
            StopAnimation("Walk");
        }
    }
    /// <summary>
    /// This is here for when ever the player needs to jump
    /// </summary>
    void Jump()
    {
        //player Jumps
        if (Input.GetKeyDown(KeyCode.Space) && jumpsMade < jumpsAllowed && jumpTimer <=0)
        {
            StopAnimation("IsLanded");
            RB.velocity = new Vector3(MoveDir.x, jumpSpeed, MoveDir.z);
            if (jumpsMade == 0)
            {
                PlayAnimation("DoubleJump");
            }
          
            jumpsMade++;
            jumpTimer = .3f;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && jumpsMade == jumpsAllowed && !DoneJumping && jumpTimer <= 0)
        {

            PlayAnimation("Dive");
            Vector3 DashDir = transform.forward * DashSpeed;
            RB.velocity = new Vector3(DashDir.x, 0, DashDir.z);
            DoneJumping = true;
            jumpTimer = .3f;
        }
        if(Input.GetKey(KeyCode.Space) && RB.velocity.y >.1f)
        {
            PlayAnimation("Jump");
        }
        if (IsGrounded())
        {
            if (DoneJumping)
            {
                PlayAnimation("IsLanded");
                DoneJumping = false;
            }
            StopAnimation("DoubleJump");
            StopAnimation("Jump");
            StopAnimation("Dive");
            jumpsMade = 0;

        }
        jumpTimer -= Time.deltaTime;
    }
    public void jumpOnEnemy()
    {
        RB.velocity = new Vector3(MoveDir.x, jumpSpeed, MoveDir.z);
        jumpsMade = 0;
    }
    public void jumpOnBouncePad(float distance)
    {
        RB.velocity = new Vector3(MoveDir.x, distance, MoveDir.z);
        jumpsMade = 0;
    }
    /// <summary>
    /// This is here for ledgeGrabbing
    /// </summary>
    public void Ledgegrabbing()
    {
        PlayAnimation("Hanging");
        RB.useGravity = false;
        RB.velocity = Vector3.zero;
        MoveDir = Vector3.zero;
        currentSpeed = 0;

        transform.rotation = new Quaternion(0, -Ledge.transform.rotation.y, 0, transform.rotation.w);
        transform.position = new Vector3(transform.position.x, Ledge.transform.position.y - LedgeOffset, transform.position.z);
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            PlayAnimation("Getup");
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("LedgeGetUp") &&
            anim.GetCurrentAnimatorStateInfo(0).length < anim.GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            StopAnimation("Getup");
            transform.position = transform.position + new Vector3(0, LedgeGetUpOffset, 0);
            transform.position += transform.forward * 2;
            LedgeGrabbing = false;
        }
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
    #endregion

    #region Other Methods
    /// <summary>
    /// This checks to see if the players grounded or not
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
    private float GetGroundAngle()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            return Vector3.Angle(Vector3.up, hit.normal);
        }
        return 0;
    }
    #endregion

}
