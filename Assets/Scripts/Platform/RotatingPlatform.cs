using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    [Tooltip("How fast the platform rotates")]
    [SerializeField]
    float RotationSpeed = 10f;

    [Tooltip("The direction platform rotates the number needs to be set to 1 or -1 to work correctly. 1 means it moves forward on the axis and -1 means it moves backwords on the axis")]
    [SerializeField]
    public Vector3 direction;

    [Tooltip("if active is true it'll work")]
    [SerializeField]
    public bool active = true;
    void LateUpdate()
    {
        if(active)
            transform.Rotate(direction, RotationSpeed * Time.deltaTime);
    }

}
