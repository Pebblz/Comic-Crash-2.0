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
    private float startingSpeed;


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
    #endregion

    #region Other Vars
    private Rigidbody RB;

    float distToGround;

    private Vector3 MoveDir;

    Transform MainCam;

    float turnSmoothVelocity;

    [HideInInspector]
    public bool isSliding;

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
        if (!LedgeGrabbing)
        {
            StopAnimation("Hanging");
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
            }
            else
            {
                PlayAnimation("Crouching");
                Crouch();
            }
        } else
        {
            Ledgegrabbing();
        }
    }
    #endregion

    #region Movement
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
        if (direction.magnitude >= 0.1f)
        {
            PlayAnimation("Walk");
            //running 
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = runSpeed;
                PlayAnimation("Run");
            }
            else
            {
                currentSpeed = startingSpeed;
                StopAnimation("Run");
            }
            //sees how much is needed to rotate to match camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + MainCam.localEulerAngles.y;

            //used to smooth the angle needed to move to avoid snapping to directions
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            if (!isSliding)
            {
                //rotate player
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                //converts rotation to direction / gives the direction you want to move in taking camera into account
                MoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                if (!IceFloor)
                {
                    RB.MovePosition(transform.position += MoveDir.normalized * currentSpeed * Time.deltaTime);
                }
                else
                {
                    RB.AddForce(MoveDir.normalized * (currentSpeed += 5) * Time.deltaTime, ForceMode.VelocityChange);
                }
            }

        }
        else
        {
            StopAnimation("Walk");
        }
        Jump();

    }
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

            if (!isSliding)
            {
                //rotate player
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                //converts rotation to direction / gives the direction you want to move in taking camera into account
                MoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;



                //moves player via velocity
                RB.AddForce(MoveDir.normalized * curRollSpeed * Time.deltaTime);
            }
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
        //player Jumps
        Jump();

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
            if (!isSliding)
            {
                //rotate player
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                //converts rotation to direction / gives the direction you want to move in taking camera into account
                MoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                RB.MovePosition(transform.position += MoveDir.normalized * CrouchSpeed * Time.deltaTime);
            }

        }
        else
        {
            StopAnimation("Walk");
        }
    }
    void Jump()
    {
        //player Jumps
        if (Input.GetKey(KeyCode.Space) && IsGrounded())
        {
            RB.velocity = new Vector3(MoveDir.x, jumpSpeed, MoveDir.z);
        }
        if (Input.GetKey(KeyCode.Space) && RB.velocity.y > .1f)
        {
            PlayAnimation("Jump");
        }
        if (IsGrounded())
        {
            StopAnimation("Jump");
        }
    }
    public void Ledgegrabbing()
    {
        PlayAnimation("Hanging");
        RB.velocity = Vector3.zero;
        RB.useGravity = false;
        transform.LookAt(new Vector3(Ledge.transform.position.x - transform.position.x,transform.position.y, Ledge.transform.position.z - transform.position.z));
        transform.position = new Vector3(transform.position.x, Ledge.transform.position.y - LedgeOffset, transform.position.z);
        if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
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
        //if (Input.GetKey(KeyCode.A))
        //{
        //    transform.Translate(Vector3.left * startingSpeed * Time.deltaTime);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    transform.Translate(-Vector3.left * startingSpeed * Time.deltaTime);
        //}
    }
    #endregion

    #region Animation
    public void PlayAnimation(string animName)
    {
        anim.SetBool(animName, true);
    }
    public void StopAnimation(string animName)
    {
        anim.SetBool(animName, false);
    }
    #endregion

    #region Other Methods
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + GroundedOffset);
    }
    public void setSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }
    #endregion

}
