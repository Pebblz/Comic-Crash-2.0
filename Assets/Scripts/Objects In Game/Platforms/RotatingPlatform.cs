using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    [SerializeField, Range(25f, 100f), Tooltip("How fast the platform rotates")]
    float RotationSpeed;

    [Tooltip("The direction the platform rotates the number needs to be set to 1 or -1 to work correctly. 1 means it moves forward on the axis and -1 means it moves backwords on the axis")]
    public Vector3 direction;

    [Tooltip("if active is true it'll work")]
    public bool active = true;

    [SerializeField] bool RotaionOnX;

    [SerializeField] bool RotaionOnZ;
    void LateUpdate()
    {
        if (active)
            transform.Rotate(direction, RotationSpeed * Time.deltaTime);
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!RotaionOnX && !RotaionOnZ)
        {
            if (collision.gameObject.tag == "Player")
                collision.collider.transform.RotateAround(transform.position, direction, RotationSpeed * Time.deltaTime);
        }
        if(RotaionOnX)
        {
            if (collision.gameObject.tag == "Player")
                collision.collider.transform.RotateAround(transform.position, new Vector3(direction.y, 0,0), RotationSpeed * Time.deltaTime);
        }
        if (RotaionOnZ)
        {
            if (collision.gameObject.tag == "Player")
                collision.collider.transform.RotateAround(transform.position, new Vector3(0, 0, direction.y), RotationSpeed * Time.deltaTime);
        }
    }
}
