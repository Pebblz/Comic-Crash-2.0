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

    [SerializeField] GameObject floatingParticalParent;

    [SerializeField]
    float ParticleRotationAngle;

    float OriginalParticleX;
    void Start()
    {
        FloatingPartical.Stop();

        NotOnGroundTimer = HowLongNeededToGlide;

        body = GetComponent<Rigidbody>();

        originalMass = body.mass;

        gravityPlane = FindObjectOfType<GravityPlane>();

        originalGravity = gravityPlane.gravity;

        movement = GetComponent<PlayerMovement>();

        photonView = GetComponent<PhotonView>();

        pause = FindObjectOfType<Pause>();

        OriginalParticleX = floatingParticalParent.GetComponent<Transform>().rotation.x;
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
                            if (movement.playerInput.x != 0 || movement.playerInput.y != 0)
                            {
                                if (FloatingPartical.isStopped)
                                {
                                    Quaternion target = Quaternion.Euler(ParticleRotationAngle, 0, 0);
                                    floatingParticalParent.transform.rotation = Quaternion.Slerp(floatingParticalParent.transform.rotation, target, Time.deltaTime * 20);
                                    FloatingPartical.Play();
                                }

                                movement.PlayAnimation("GlidingForward");
                                movement.StopAnimation("GlidingIdle");
                            }
                            if (movement.playerInput.x == 0 && movement.playerInput.y == 0)
                            {

                                if (FloatingPartical.isStopped)
                                {
                                    Quaternion target = Quaternion.Euler(OriginalParticleX, 0, 0);
                                    floatingParticalParent.transform.rotation = Quaternion.Slerp(floatingParticalParent.transform.rotation, target, Time.deltaTime * 20);
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
                        unSetGravity();
                        movement.PlayAnimation("Falling");
                        movement.StopAnimation("GlidingIdle");
                        movement.StopAnimation("GlidingForward");
                    }
                }
                else
                {
                    if (FloatingPartical.isPlaying)
                    {
                        FloatingPartical.Stop();
                        FloatingPartical.Clear();
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
