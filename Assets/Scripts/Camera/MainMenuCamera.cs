using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField, Range(1f, 20f), Tooltip("How fast the camera rotates")]
    float rotationSpeed;

    [SerializeField, Tooltip("The direction the camera rotates the number needs to be set to 1 or -1 to work correctly. 1 means it moves forward on the axis and -1 means it moves backwords on the axis")]
    Vector3 direction;
    void Update()
    {
        transform.Rotate(direction, rotationSpeed * Time.deltaTime);
    }
}
