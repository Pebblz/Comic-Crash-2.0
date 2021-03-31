using UnityEngine;
public class Camera : MonoBehaviour
{
    #region Third Person Vars
    [Tooltip("This is what the camera will be focusing on in game")]
    public Transform target;

    [SerializeField, Range(1f, 10f), Tooltip("The starting distance away from the player")]
    float distance = 5.0f;

    [SerializeField, Range(100f, 160f), Tooltip("How fast the camera can go left to right")]
    float xSpeed = 120.0f;

    [SerializeField, Range(100f, 160f), Tooltip("How fast the camera can go up and down")]
    float ySpeed = 120.0f;

    [SerializeField, Range(-45f, 45f), Tooltip("How far the camera can rotate down")]
    float yMinLimit = -20f;

    [SerializeField, Range(75f, 90f), Tooltip("How far the camera can rotate up")]
    float yMaxLimit = 80f;

    [SerializeField, Range(.5f, 5f), Tooltip("The min distance for scrolling in")]
    float distanceMin = .5f;

    [SerializeField, Range(10f, 20f), Tooltip("The max distance away for scrolling out")]
    float distanceMax = 15f;

    float x = 0.0f;
    float y = 0.0f;

    [SerializeField, Range(-100, 10)]
    float fallLookAtPosition;
    #endregion

    [HideInInspector, Tooltip("If this is true, it'll the camera will be in third person if it's not true it'll be in first person")]
    public bool thirdPersonCamera = true;

    private Rigidbody rigidbody;

    private float rotationOnX;

    [SerializeField, Range(45f, 180f), Tooltip("The sensitivity of the camera when the player's in first person mode")]
    float MouseSensitivity = 90;

    [SerializeField] GameObject CrossHair;

    Vector3 startPos;
    #region MonoBehaviours
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
        }
        else
        {
            FirstPersonCamera();
        }
    }
    #endregion
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
    #region camera styles
    void ThirdPersonCamera()
    {
        CrossHair.SetActive(false);
        if (transform.parent != null)
        {
            transform.parent = null;
        }
        if (target)
        {
            if (transform.position.y > fallLookAtPosition)
            {
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);

                Quaternion rotation = Quaternion.Euler(y, x, 0);

                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

                RaycastHit hit;

                if (Physics.Linecast(target.position, transform.position, out hit))
                {
                    if (hit.transform.gameObject.layer != 9)
                    {
                        distance -= hit.distance;
                    }
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
        //this makes the camera always in front of the player 
        transform.position = target.position + target.transform.forward + new Vector3(0, .4f, 0);
        CrossHair.SetActive(true);
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
    #endregion
}
