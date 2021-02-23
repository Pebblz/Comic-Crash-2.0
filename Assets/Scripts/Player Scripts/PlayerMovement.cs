using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
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


    private Rigidbody RB;

    float distToGround;

    private Vector3 MoveDir;

    Transform MainCam;

    float turnSmoothVelocity;

    [HideInInspector]
    public bool isSliding;

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
    #endregion
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
    public void setSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }
}
