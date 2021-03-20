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
    void LateUpdate()
    {
        if (active)
            transform.Rotate(direction, RotationSpeed * Time.deltaTime);
    }

}
