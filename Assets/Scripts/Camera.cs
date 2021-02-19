using UnityEngine;

public class Camera : MonoBehaviour
{
    #region Third Person Vars
    [Tooltip("This is what the camera will be focusing on in game")]
    public Transform target;
    [Tooltip("The starting distance away from the player")]
    [SerializeField]
    float distance = 5.0f;
    [Tooltip("How fast the camera can go left to right")]
    [SerializeField]
    float xSpeed = 120.0f;
    [Tooltip("How fast the camera can go up and down")]
    [SerializeField]
    float ySpeed = 120.0f;
    [Tooltip("How far the camera can rotate down")]
    [SerializeField]
    float yMinLimit = -20f;
    [Tooltip("How far the camera can rotate up")]
    [SerializeField]
    float yMaxLimit = 80f;
    [Tooltip("The min distance for scrolling in")]
    [SerializeField]
    float distanceMin = .5f;
    [Tooltip("The max distance away for scrolling out")]
    [SerializeField]
    float distanceMax = 15f;

    float x = 0.0f;
    float y = 0.0f;
    #endregion

    [Tooltip("If this is true, it'll the camera will be in third person if it's not true it'll be in first person")]
    public bool thirdPersonCamera = true;

    private Rigidbody rigidbody;

    private float rotationOnX;

    [Tooltip("The sensitivity of the camera when the player's in first person mode")]
    [SerializeField]
    float MouseSensitivity = 90;

    Vector3 startPos;
    // Use this for initialization
    void Start()
    {
        startPos = transform.position;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
    }

    void LateUpdate()
    {
        if (thirdPersonCamera)
        {
            ThirdPersonCamera();
        } else
        {
            FirstPersonCamera();
        }
    }
    public void ResetCamera()
    {
        transform.position = startPos;
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
    void ThirdPersonCamera()
    {
        if(transform.parent != null)
        {
            transform.parent = null;
        }
        if (target)
        {
            if (transform.position.y > -10)
            {
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);

                Quaternion rotation = Quaternion.Euler(y, x, 0);

                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

                RaycastHit hit;
                if (Physics.Linecast(target.position, transform.position, out hit))
                {
                    distance -= hit.distance;
                }
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target.position;

                transform.rotation = rotation;
                transform.position = position;
            }
            else
            {
                transform.LookAt(target);
            }
        }
    }
    void FirstPersonCamera()
    {
        transform.position = target.position + new Vector3(0, .5f, 0);
        if (transform.parent == null)
        {
            transform.SetParent(target);
        }
        
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * MouseSensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * MouseSensitivity;

        rotationOnX -= mouseY;
        rotationOnX = Mathf.Clamp(rotationOnX, -90, 90);
        transform.localEulerAngles = new Vector3(rotationOnX, 0, 0);

        target.Rotate(Vector3.up * mouseX);
    }
}
