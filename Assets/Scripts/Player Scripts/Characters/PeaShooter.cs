using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaShooter : MonoBehaviour
{
    private GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            camera.GetComponent<Camera>().thirdPersonCamera = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            camera.GetComponent<Camera>().thirdPersonCamera = true;
        }
    }
}
