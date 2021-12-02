using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraLookAt : MonoBehaviour
{

    GameObject player;
    // Start is called before the first frame update

    CinemachineFreeLook cam;
    private void Start()
    {
        this.cam = GetComponent<CinemachineFreeLook>();
        player = GameObject.FindGameObjectWithTag("Player");
        cam.Follow = player.transform;
        cam.LookAt = player.transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            cam.Follow = player.transform;
            cam.LookAt = player.transform;

        }
    }
}
