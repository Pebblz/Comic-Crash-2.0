using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region Speed Vars
    [Header("Movement speeds")]
    [SerializeField]
    [Range(5f, 20f)]
    private int startingSpeed;

    [HideInInspector]
    public float currentSpeed;

    [Range(10f, 30f)]
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
    #endregion

    #region MonoBehaviours
    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y - .1f;
        RB = GetComponent<Rigidbody>();
        MainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Roll)
        {
            regularMovement();
        }
        else
        {
            rolling();
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

        if (direction.magnitude >= 0.1f)
        {
            //running 
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = startingSpeed;
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

                RB.MovePosition(transform.position += MoveDir.normalized * currentSpeed * Time.deltaTime);
            }

        }
        //player Jumps
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !isSliding)
        {
            RB.velocity = new Vector3(direction.x, jumpSpeed, direction.z);
        }
    }
    void rolling()
    {
        RB.constraints = RigidbodyConstraints.None;

        float H = Input.GetAxisRaw("Horizontal");
        float V = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(H, 0, V).normalized;
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

                //player Jumps
                if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
                {
                    RB.velocity = new Vector3(MoveDir.x, jumpSpeed, MoveDir.z);
                }

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

    }
    #endregion

    #region Other Methods
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
    public void setSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }
    #endregion

}
