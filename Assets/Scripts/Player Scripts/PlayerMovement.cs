using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float speed;
    Rigidbody RB;
    float distToGround;
    [SerializeField]
    float jumpSpeed;
    Vector3 MoveDir;
    [SerializeField]
    float turnSmoothTime = .1f;
    Transform MainCam;
    float turnSmoothVelocity;
    // Start is called before the first frame update
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
            //sees how much is needed to rotate to match camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + MainCam.localEulerAngles.y;

            //used to smooth the angle needed to move to avoid snapping to directions
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //rotate player
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            //converts rotation to direction / gives the direction you want to move in taking camera into account
            MoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            RB.MovePosition(transform.position += MoveDir.normalized * speed * Time.deltaTime);
        }
        //player Jumps
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            RB.velocity = new Vector3(direction.x, jumpSpeed, direction.z);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
