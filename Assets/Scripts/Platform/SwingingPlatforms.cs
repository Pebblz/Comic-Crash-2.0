using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingPlatforms : MonoBehaviour
{
    [Tooltip("The speed that the platforms moves")]
    [SerializeField]
    float speed = 2.0f;

    [Tooltip("The max distance the platform will go")]
    [SerializeField]
    float distance = 30;

    private float direction = 1;

    private Quaternion startRot;
    private void Start()
    {
        startRot = transform.rotation;  
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion a = startRot;
        a.x += direction * (distance * Mathf.Sin(Time.time * speed));
        transform.rotation = a;
    }
}
