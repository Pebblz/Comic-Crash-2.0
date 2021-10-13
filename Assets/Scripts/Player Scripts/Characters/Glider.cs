﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Photon.Realtime;
using Photon.Pun;
public class Glider : MonoBehaviour
{
    GravityPlane gravityPlane;

    Rigidbody body;

    float originalMass;

    [SerializeField]
    float glidingMass;

    [SerializeField]
    float glidingGravity;

    float originalGravity;

    PlayerMovement movement;

    PhotonView photonView;

    Pause pause;

    float NotOnGroundTimer;

    [SerializeField]
    float HowLongNeededToGlide = .3f;
    void Start()
    {
        NotOnGroundTimer = HowLongNeededToGlide;

        body = GetComponent<Rigidbody>();

        originalMass = body.mass;

        gravityPlane = FindObjectOfType<GravityPlane>();

        originalGravity = gravityPlane.gravity;

        movement = GetComponent<PlayerMovement>();

        photonView = GetComponent<PhotonView>();

        pause = FindObjectOfType<Pause>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (!pause.isPaused)
            {
                if (!movement.OnGround)
                {
                    if (InputManager.GetButton("Jump"))
                    {
                        NotOnGroundTimer -= Time.deltaTime;
                        if (NotOnGroundTimer <= 0)
                        {
                            SetGravity();
                        }
                    }
                    else
                    {
                        unSetGravity();
                    }
                }
                else
                {
                    unSetGravity();
                    NotOnGroundTimer = HowLongNeededToGlide;
                }
            }
        }
    }

    void SetGravity()
    {
        gravityPlane.gravity = glidingGravity;
        body.mass = glidingMass;
    }
    void unSetGravity()
    {
        gravityPlane.gravity = originalGravity;
        body.mass = originalMass;
    }
    private void OnDestroy()
    {
        unSetGravity();
    }
}