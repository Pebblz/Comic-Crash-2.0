﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
public class PlayerGhost : MonoBehaviour
{
    private float ghostPositionY;

    PlayerMovement player;
    Camera cam;

    [SerializeField]
    float maxSpeed = 5;
    [SerializeField]
    float SmoothTime = .5f;
    Vector3 vel;
    bool once;
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        var temp = GameObject.Find("FreeLookCamera").GetComponent<CinemachineFreeLook>();
        temp.Follow = gameObject.transform;
        temp.LookAt = gameObject.transform;
        temp.GetRig(0).LookAt = gameObject.transform;
        temp.GetRig(1).LookAt = gameObject.transform;
        temp.GetRig(2).LookAt = gameObject.transform;
    }
    private void Update()
    {
        if (player != null)
        {
            if (!player.OnGround)
            {
                if (!once)
                {
                    OnLeaveGround();
                    once = true;
                }
            }
            else
            {
                once = false;
            }
        }
        else
        {
            player = PhotonFindCurrentClient().GetComponent<PlayerMovement>();
        }
    }
    void OnLeaveGround()
    {
        // update Y for behavior 3
        ghostPositionY = player.transform.position.y;
    }
    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 ViewPos = cam.WorldToViewportPoint(player.transform.position + player.velocity * Time.deltaTime);

            // behavior 2
            if (ViewPos.y > 0.85f || ViewPos.y < 0.3f)
            {
                ghostPositionY = player.transform.position.y;
            }
            // behavior 4
            else if (player.OnGround)
            {
                ghostPositionY = player.transform.position.y;
            }    // behavior 5

            var desiredPosition = new Vector3(player.transform.position.x, ghostPositionY, player.transform.position.z); 
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref vel, SmoothTime, maxSpeed);
            
        }
        else
        {
            player = PhotonFindCurrentClient().GetComponent<PlayerMovement>();
        }
    }
    GameObject PhotonFindCurrentClient()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject g in players)
        {
            if (g.GetComponent<PhotonView>().IsMine)
                return g;
        }
        return null;
    }
}