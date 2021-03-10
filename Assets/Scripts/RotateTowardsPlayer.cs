using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsPlayer : MonoBehaviour
{
    public GameObject camera;
    void Update()
    {
        if (camera == null)
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        else
        {
            Vector3 temp = camera.transform.forward;
            temp.y = 90;
            transform.rotation = Quaternion.LookRotation(-temp);
        }
    }
}
