using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingPlatforms : MonoBehaviour
{
    [Tooltip("The speed that the platforms moves")]
    [Range(.5f, 2f)]
    [SerializeField]
    float speed;

    [Tooltip("The max distance the platform will go")]
    [Range(.2f, 1f)]
    [SerializeField]
    float distance;

    private Quaternion startRot;
    private void Start()
    {
        startRot = transform.rotation;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion a = startRot;
        a.x += 1 * (distance * Mathf.Sin(Time.time * speed));
        transform.rotation = a;
    }
}
