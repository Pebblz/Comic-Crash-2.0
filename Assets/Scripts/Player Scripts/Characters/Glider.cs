using System.Collections;
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

    [SerializeField] ParticleSystem FloatingPartical;

    [SerializeField] ParticleSystem FloatingParticleRotated;
    void Start()
    {
        FloatingPartical.Stop();
        FloatingParticleRotated.Stop();

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
                if (!movement.OnGround && !movement.InWater)
                {
                    if (InputManager.GetButton("Jump"))
                    {
                        NotOnGroundTimer -= Time.deltaTime;
                        if (NotOnGroundTimer <= 0)
                        {
                            SetGravity();
                            if (movement.playerInput.x != 0 || movement.playerInput.y != 0)
                            {
                                if (FloatingPartical.isPlaying)
                                {
                                    FloatingPartical.Stop();
                                    FloatingPartical.Clear();
                                }
                                if (FloatingParticleRotated.isStopped)
                                {
                                    FloatingParticleRotated.Play();
                                }

                                movement.PlayAnimation("GlidingForward");
                                movement.StopAnimation("GlidingIdle");
                            }
                            if (movement.playerInput.x == 0 && movement.playerInput.y == 0)
                            {
                                if (FloatingParticleRotated.isPlaying)
                                {
                                    FloatingParticleRotated.Stop();
                                    FloatingParticleRotated.Clear();
                                }
                                if (FloatingPartical.isStopped)
                                {
                                    FloatingPartical.Play();
                                }
                                movement.PlayAnimation("GlidingIdle");
                                movement.StopAnimation("GlidingForward");
                            }
                        }
                    }
                    else
                    {
                        if (FloatingPartical.isPlaying)
                        {
                            FloatingPartical.Stop();
                            FloatingPartical.Clear();
                        }
                        if (FloatingParticleRotated.isPlaying)
                        {
                            FloatingParticleRotated.Stop();
                            FloatingParticleRotated.Clear();
                        }
                        unSetGravity();
                        movement.PlayAnimation("Falling");
                        movement.StopAnimation("GlidingIdle");
                        movement.StopAnimation("GlidingForward");
                    }
                }
                if (movement.InWater || movement.OnGround)
                {
                    if (FloatingPartical.isPlaying)
                    {
                        FloatingPartical.Stop();
                        FloatingPartical.Clear();
                    }
                    if (FloatingParticleRotated.isPlaying)
                    {
                        FloatingParticleRotated.Stop();
                        FloatingParticleRotated.Clear();
                    }
                    unSetGravity();
                    movement.StopAnimation("GlidingIdle");
                    movement.StopAnimation("GlidingForward");
                    NotOnGroundTimer = HowLongNeededToGlide;
                }
            }
        }
    }

    void SetGravity()
    {
        movement.Gliding = true;
        gravityPlane.gravity = glidingGravity;
        body.mass = glidingMass;
    }
    void unSetGravity()
    {
        movement.Gliding = false;
        gravityPlane.gravity = originalGravity;
        body.mass = originalMass;
    }
    private void OnDestroy()
    {
        unSetGravity();
    }
}
