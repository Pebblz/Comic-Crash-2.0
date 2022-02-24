﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class BossOne : MonoBehaviour
{
    #region Vars
    [SerializeField]
    private float walkRotSpeed = 2, chaseRotSpeed = 3, idleMoveSpeed, chaseSpeed, chargeSpeed,
        timeTillIdle, playerSeenRange, chargeCooldown, chargeDuration,
        firstSeenPlayer, distAwayToCharge = 8, stunDuration, maxIFrameTime;

    int damage = 1, health = 3;

    [SerializeField, Tooltip("The different idle states")]
    WaysToIdle idleWays = WaysToIdle.StandAtPoint;

    [SerializeField, Tooltip("Should the enemy go back to starttign rotation when going back to idle")]
    bool realignWithStartingRot;

    GameObject chasedPlayer;

    Vector3 lastSeenPlayerPos;

    Animator anim;

    List<GameObject> detectedPlayers = new List<GameObject>();

    EnemyDetection Detection;

    Vector3 startingPoint;

    Rigidbody rb;

    [SerializeField]
    List<GameObject> waypoints = new List<GameObject>();

    Quaternion startingRot;

    float chargeCooldownTimer, chargeDurationTimer, firstSeenPlayerTimer, stunTimer,
        IFrameTimer;

    bool charging, stunned, dead;

    PhotonView photonView;

    #endregion

    private void Start()
    {
        anim = GetComponent<Animator>();
        Detection = GetComponent<EnemyDetection>();
        startingPoint = transform.position;
        startingRot = transform.rotation;
        rb = GetComponent<Rigidbody>();
        chargeDurationTimer = chargeDuration;
        firstSeenPlayerTimer = firstSeenPlayer;
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (!charging && !stunned && !dead)
            {
                if (detectedPlayers.Count > 0)
                {
                    anim.SetBool("Walking", true);
                    ChasePlayer();
                    firstSeenPlayerTimer -= Time.deltaTime;
                }
                else
                {
                    detectedPlayers = Detection.FindAllPlayersInLevel();
                    chasedPlayer = Detection.FindClosestPlayer(detectedPlayers);
                }
                chargeCooldownTimer -= Time.deltaTime;
            }
            else
            {
                if (charging)
                {
                    anim.SetBool("Charge", true);
                    if (anim.GetAnimatorTransitionInfo(0).IsName("Start_Charge -> Charging") || anim.GetCurrentAnimatorStateInfo(0).IsName("Charging"))
                    {
                        ChargeAtPlayer();
                        chargeDurationTimer -= Time.deltaTime;
                        if (chargeDurationTimer <= 0)
                        {
                            ResetCharge();
                        }
                    }
                }
                if (stunned)
                {
                    anim.SetBool("Charge", false);
                    anim.SetBool("Stunned", true);
                    if (anim.GetAnimatorTransitionInfo(0).IsName("Stun 1 -> Stun 2") || anim.GetCurrentAnimatorStateInfo(0).IsName("Stun 2"))
                    {
                        stunTimer -= Time.deltaTime;
                        if (stunTimer <= 0)
                        {
                            anim.SetBool("Stunned", false);
                            anim.SetBool("UnStunned", true);
                        }
                    }
                    if (anim.GetAnimatorTransitionInfo(0).IsName("Stun 3 -> Idle"))
                    {
                        ResetCharge();
                        anim.SetBool("UnStunned", false);
                        anim.SetBool("Stunned", false);
                        stunned = false;
                    }
                }
            }
            //plays the dead anim
            if (health <= 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
            {
                anim.SetBool("Dead", true);
                dead = true;
            }
            //turns off the enemy when dead's over
            if (anim.GetAnimatorTransitionInfo(0).IsName("Dead -> Walk"))
            {
                anim.SetBool("Dead", false);
                //do this after death anim
                gameObject.SetActive(false);
            }
            IFrameTimer -= Time.deltaTime;
        }
    }

    private void ChasePlayer()
    {
        Vector3 ChasedPlayersPos = new Vector3(chasedPlayer.transform.position.x, transform.position.y, chasedPlayer.transform.position.z);

        //Rotates slowly towards player
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                          Quaternion.LookRotation(ChasedPlayersPos - transform.position),
                                          chaseRotSpeed * Time.deltaTime);

        //Moves enemy towards player
        if (Vector3.Distance(transform.position, chasedPlayer.transform.position) > 2.5f && !charging)
            rb.velocity = (transform.forward * chaseSpeed) + new Vector3(0, rb.velocity.y, 0);
        //charges at player
        if (Vector3.Distance(transform.position, chasedPlayer.transform.position) <= distAwayToCharge && chargeCooldownTimer <= 0 &&
            firstSeenPlayerTimer <= 0)
            charging = true;
        //if he's on top of the player i just want him to ignore the cooldown timer and attack
        if (Vector3.Distance(transform.position, chasedPlayer.transform.position) <= 2.5f && Dot(.95f, ChasedPlayersPos))
            charging = true;
    }

    void ChargeAtPlayer()
    {
        rb.velocity = (transform.forward + new Vector3(0, rb.velocity.y, 0)) * chargeSpeed;
    }

    void DamagePlayer(GameObject AttackedPlayer)
    {
        PlayerHealth Player = AttackedPlayer.GetComponent<PlayerHealth>();
        if (Player != null)
        {
            Vector3 dir = AttackedPlayer.transform.position - transform.position;
            //AttackedPlayer.GetComponent<PlayerMovement>().Knockback(dir,-200,2);
            //makes sure spider doesn't chase player when he respawns
            Player.HurtPlayer(damage);
            if (AttackedPlayer.GetComponent<PlayerDeath>().isdead)
            {
                if (detectedPlayers.Contains(AttackedPlayer))
                {
                    detectedPlayers.Remove(AttackedPlayer);
                }
                chasedPlayer = null;
            }
        }

    }

    bool Dot(float distAway, Vector3 OtherPos)
    {

        //the direction of the last seen pos 
        Vector3 dir = (OtherPos - transform.position).normalized;
        //this sees if the enemies looking at the last seen pos
        float dot = Vector3.Dot(dir, transform.forward);

        if (dot > distAway)
            return true;
        else
            return false;
    }

    void ResetCharge()
    {
        anim.SetBool("Charge", false);
        chasedPlayer = Detection.FindClosestPlayer(detectedPlayers);
        chargeCooldownTimer = chargeCooldown;
        chargeDurationTimer = chargeDuration;
        charging = false;
    }

    private void OnTriggerStay(Collider col)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Charging") && col.gameObject.tag == "Player")
        {
            //also knockback player
            DamagePlayer(col.gameObject);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Charging") && col.gameObject.tag != "Player"
            && !stunned && col.gameObject.tag != "PlayerPunch" && col.gameObject.tag != "Shot")
        {
            if (gameObject.transform.parent == null || gameObject.transform.parent != this.gameObject)
            {
                //make here for stunning the enemy when hitting a wall
                stunTimer = stunDuration;
                detectedPlayers = Detection.GetPlayersInSight();
                stunned = true;
            }
        }
        if (photonView.IsMine)
        {
            //if (col.gameObject.GetComponent<Bullet>() && IFrameTimer <= 0 ||
            //col.gameObject.tag == "PlayerPunch" && IFrameTimer <= 0)
            //{
            //    health -= 1;
            //    IFrameTimer = maxIFrameTime;
            //}
        }
    }

    public void reset_data()
    {
        health = 3;
        detectedPlayers.Clear();
        chasedPlayer = null;
        transform.position = startingPoint;
        transform.rotation = startingRot;
        lastSeenPlayerPos = Vector3.zero;
        chargeDurationTimer = chargeDuration;
        stunned = false;
        charging = false;
        dead = false;
    }

    enum WaysToIdle
    {
        StandAtPoint,
        WayPoint
    }
}
