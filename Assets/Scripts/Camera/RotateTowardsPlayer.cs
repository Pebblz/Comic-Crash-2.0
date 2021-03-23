using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsPlayer : MonoBehaviour
{
    public GameObject camera;
    [SerializeField] bool textMesh;
    void Update()
    {
        if (camera == null)
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        else
        {
            Vector3 temp = camera.transform.forward;

            if (!textMesh)
            {
                temp.y = 90;
                transform.rotation = Quaternion.LookRotation(-temp);
            }
            else if (textMesh)
            {
                transform.rotation = Quaternion.LookRotation(temp);
            }
        }
    }
}
