using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFishRotation : MonoBehaviour
{
    [SerializeField] float speed;
    void FixedUpdate()
    {
        transform.Rotate(Vector3.right * speed * Time.deltaTime);
    }
}
