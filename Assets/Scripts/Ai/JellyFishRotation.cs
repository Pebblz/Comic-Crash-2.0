using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFishRotation : MonoBehaviour
{
    [SerializeField] float speed;
    void FixedUpdate()
    {
        transform.Rotate(Vector3.right * speed * Time.deltaTime);
        //Quaternion q = Quaternion.FromToRotation(transform.GetChild(0).up, Vector3.up) * transform.GetChild(0).rotation;
        //transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, q, Time.deltaTime * speed);
    }
}
