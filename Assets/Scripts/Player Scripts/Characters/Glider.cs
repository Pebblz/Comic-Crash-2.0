using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Photon.Realtime;
using Photon.Pun;
public class Glider : MonoBehaviour
{
    GravityPlane gravityPlane;

    [SerializeField]
    float glidingGravity;

    float originalGravity;

    PlayerMovement movement;

    PhotonView photonView;

    Pause pause;

    float NotOnGroundTimer;

    float HowLongNeededToGlide = .5f;
    void Start()
    {
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
                if (InputManager.GetButtonDown("Jump"))
                {
                    NotOnGroundTimer += Time.deltaTime;
                    if (!movement.OnGround && NotOnGroundTimer >= HowLongNeededToGlide)
                    {
                        SetGravity();
                    }
                }
                else
                {
                    unSetGravity();
                    NotOnGroundTimer = 0;
                }
            }
        }
    }

    void SetGravity()
    {
        gravityPlane.gravity = glidingGravity;
    }
    void unSetGravity()
    {
        gravityPlane.gravity = originalGravity;
    }
    private void OnDestroy()
    {
        unSetGravity();
    }
}
