using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Tooltip("How fast the camera rotates")]
    [Range(1f, 20f)]
    [SerializeField]
    float rotationSpeed;

    [Tooltip("The direction the camera rotates the number needs to be set to 1 or -1 to work correctly. 1 means it moves forward on the axis and -1 means it moves backwords on the axis")]
    public Vector3 direction;
    void Update()
    {
        transform.Rotate(direction, rotationSpeed * Time.deltaTime);
    }
}
