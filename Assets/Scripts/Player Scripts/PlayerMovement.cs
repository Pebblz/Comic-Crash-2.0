using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

    #region Speed Vars
    [Header("Movement speeds")]
    [SerializeField]
    [Range(1f, 20f)]
    private float WalkSpeed;

    [SerializeField]
    [Range(.001f, 2f)]
    private float SpeedAcceleration;

    [SerializeField]
    [Range(.001f, 2f)]
    private float SpeedDeceleration;

    [SerializeField]
    [Range(1f, 10f)]
    private int CrouchSpeed;

    [HideInInspector]
    public float currentSpeed;

    [Range(5f, 30f)]
    [SerializeField]
    private float runSpeed;

    [Range(1f, 10f)]
    [SerializeField]
    float jumpSpeed;

    [Range(.1f, .5f)]
    [SerializeField]
    float turnSmoothTime = .1f;

    [HideInInspector]
    float curRollSpeed = 0;

    [Range(100f, 120f)]
    [SerializeField]
    float maxRollSpeed;

    [Tooltip("How fast it'll Accelerate")]
    [Range(20f, 40f)]
    [SerializeField]
    float RollAcceleration;

    [Tooltip("How fast it'll Decelerate")]
    [Range(.1f, 1f)]
    [SerializeField]
    float RollDeceleration;

    [Tooltip("Amount of jumps allowed")]
    [Range(0, 5)]
    [SerializeField]
    int jumpsAllowed;

    [Tooltip("The speed of the dash")]
    [Range(1f, 20f)]
    [SerializeField]
    float DashSpeed;

    [Tooltip("The speed of the Slide")]
    [Range(1f, 20f)]
    [SerializeField]
    float SlideSpeed;

    [Tooltip("The speed of the Grapple")]
    [Range(1f, 10f)]
    [SerializeField]
    float GrappleSpeed;

    private int jumpsMade;
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



    private bool Grounded;

    private Vector3 ColliderScale;

    private Vector3 ColliderCenter;

    [Header("Offsets")]

    [SerializeField]
    [Range(.01f, 3f)]
    float GroundedOffset;

    [SerializeField]
    [Range(0, .5f)]
    float CrouchOffsetY;

    [HideInInspector]
    public GameObject Ledge;


    [SerializeField]
    [Range(.01f, 1f)]
    float LedgeOffset;

    [SerializeField]
    [Range(.5f, 3f)]
    float LedgeGetUpOffset;


    [HideInInspector]
    public bool IceFloor;
    [HideInInspector]
    public bool InGrapple;

    private bool DoneJumping;
    #endregion

    #region MonoBehaviours
    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;
        RB = GetComponent<Rigidbody>();
        MainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        anim = GetComponent<Animator>();
        if (GetComponent<BoxCollider>() != null)
        {
            ColliderScale = GetComponent<BoxCollider>().size;
            ColliderCenter = GetComponent<BoxCollider>().center;
        }
    }

    // Update is called once per frame
    void Update()
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
                    }
                }
                else if (LedgeGrabbing)
                {
                    Ledgegrabbing();
                }
            }
            else
            {
                PlayAnimation("Grapple");
                GrappleMovement();
            }
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
            PlayAnimation("Run");
            if (direction.magnitude >= 0.1f)
            {
                if (currentSpeed < runSpeed)
                {
                    currentSpeed += SpeedAcceleration;
                }
            }
        }
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            StopAnimation("Run");
            if (direction.magnitude >= 0.1f)
            {
                if (currentSpeed < WalkSpeed)
                {
                    currentSpeed += SpeedAcceleration;
                }
                else if (currentSpeed > WalkSpeed)
                {
                    currentSpeed -= SpeedAcceleration;
                }
            }
            else
            {
                if (currentSpeed > 0)
                {
                    currentSpeed -= SpeedDeceleration;
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

            StopAnimation("Walk");
            StopAnimation("Run");
        }
        //this is here just incase it gets set to a negative 
        if (currentSpeed < 0)
        {
            currentSpeed = 0;
        }

        if (!IceFloor)
        {
            //this is here so when he walks he just flips backwords when he wants to move backwords
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                RB.MovePosition(transform.position += MoveDir.normalized * currentSpeed * Time.deltaTime);
            }
            //this is here so he can do a little loop when he goes from running forward to running backword
            if (Input.GetKey(KeyCode.LeftShift) && direction.magnitude >= 0.1f)
            {
                RB.MovePosition(transform.position += transform.forward * currentSpeed * Time.deltaTime);
            }
        }
        else
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
        if (Input.GetKeyDown(KeyCode.Space) && jumpsMade < jumpsAllowed)
        {
            StopAnimation("IsLanded");
            RB.velocity = new Vector3(MoveDir.x, jumpSpeed, MoveDir.z);
            jumpsMade += 1;
            if (jumpsMade == 1)
            {
                PlayAnimation("DoubleJump");
            }

        }
        else if (Input.GetKey(KeyCode.Space) && jumpsMade == jumpsAllowed && !DoneJumping)
        {

            PlayAnimation("Dive");
            Vector3 DashDir = transform.forward * DashSpeed;
            RB.velocity = new Vector3(DashDir.x, 0, DashDir.z);
            DoneJumping = true;
        }

        if (Input.GetKey(KeyCode.Space) && RB.velocity.y > .1f)
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
    }
    /// <summary>
    /// This is here for ledgeGrabbing
    /// </summary>
    public void Ledgegrabbing()
    {
        PlayAnimation("Hanging");
        RB.velocity = Vector3.zero;
        RB.useGravity = false;
        transform.LookAt(new Vector3(Ledge.transform.position.x - transform.position.x, transform.position.y, Ledge.transform.position.z - transform.position.z));
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
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + GroundedOffset);
    }
    #endregion

}
