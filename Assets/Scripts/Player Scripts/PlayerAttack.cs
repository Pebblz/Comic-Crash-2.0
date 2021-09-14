﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] int AmountOfAttacks;
    [SerializeField] GameObject[] PunchHitBoxes;
    [SerializeField] float[] AttackTimers = new float[3];

    private Animator anim;
    private int AttacksPreformed = 1;
    private float TimeTillnextAttack;
    private float TimeTillAttackReset;
    private bool AttackAgian;
    [HideInInspector] public bool CanAttack;
    bool AirAttacked;
    HandMan handman;
    Rigidbody body;
    PlayerMovement movement;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
        if (GetComponent<HandMan>())
            handman = GetComponent<HandMan>();
    }

    // Update is called once per frame
    void Update()
    {
        #region Punch
        //this is so if the players in any of
        //the jump animation you can't attack
        if (!anim.GetBool("Jump"))
        { // Ground Punch
            if (handman == null)
            {
                if (Input.GetButtonDown("Fire1") && AttacksPreformed == 1 && TimeTillnextAttack <= 0
                   && !Input.GetKey(KeyCode.C))
                {
                    punch(AttacksPreformed);
                }
                //this basically queues up the next attack for the player
                if (Input.GetButtonDown("Fire1") && AmountOfAttacks >= AttacksPreformed && !Input.GetKey(KeyCode.C) && TimeTillnextAttack <= 0)
                {
                    AttackAgian = true;
                }
                //this waits for the animation to be done before going to the next punch
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Punch" + (AttacksPreformed - 1)) &&
                    anim.GetCurrentAnimatorStateInfo(0).normalizedTime < .90f && AttackAgian)
                {
                    punch(AttacksPreformed);
                }
            }
            else
            {
                if (!handman.isHoldingOBJ)
                {
                    if (Input.GetButtonDown("Fire1") && AttacksPreformed == 1 &&
                        TimeTillnextAttack <= 0 && !Input.GetKey(KeyCode.C))
                    {
                        punch(AttacksPreformed);
                    }
                    //this basically queues up the next attack for the player
                    if (Input.GetButtonDown("Fire1") && AmountOfAttacks >= AttacksPreformed && !Input.GetKey(KeyCode.C) && TimeTillnextAttack <= 0)
                    {
                        AttackAgian = true;
                    }
                    //this waits for the animation to be done before going to the next punch
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Punch" + (AttacksPreformed - 1)) &&
                        anim.GetCurrentAnimatorStateInfo(0).normalizedTime < .90f && AttackAgian)
                    {
                        punch(AttacksPreformed);
                    }
                }
            }
        }
        else
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Dive") || !anim.GetCurrentAnimatorStateInfo(0).IsName("GPFalling"))
            {
                // Air Punch
                if (handman == null)
                {
                    if (Input.GetButtonDown("Fire1") && AttacksPreformed == 1 && TimeTillnextAttack <= 0)
                    {
                        PunchAir();
                    }
                }
                else
                {
                    if (!handman.isHoldingOBJ)
                    {
                        if (Input.GetButtonDown("Fire1") && AttacksPreformed == 1 && TimeTillnextAttack <= 0)
                        {
                            PunchAir();
                        }
                    }
                }
            }
        }
        #endregion
        //this is so the player can't swing around like a crazy person and kill everything around him
        if (TimeTillAttackReset > 0 && movement.OnGround)
        {
            movement.enabled = false;
            body.velocity = Vector3.zero;
        }

        if (TimeTillAttackReset > 0 && !movement.OnGround)
        {
            body.velocity = new Vector3(body.velocity.x, -1, body.velocity.y);
        }

        if (TimeTillAttackReset <= 0 && movement.OnGround)
        {
            StopAnimationInt("Attack");
            StopAnimationBool("AirAttack");
            movement.enabled = true;
            AttacksPreformed = 1;
        }
        if (TimeTillAttackReset <= 0 && !movement.OnGround && AirAttacked)
        {
            StopAnimationBool("AirAttack");
            AirAttacked = false;
            movement.PlayFallingAnimation();
        }
        TimeTillAttackReset -= Time.deltaTime;
        TimeTillnextAttack -= Time.deltaTime;
    }
    void PunchAir()
    {
        Instantiate(PunchHitBoxes[0], transform.position + new Vector3(0, .6f, 0) + transform.forward * 1.1f, Quaternion.identity);
        PlayAnimation("AirAttack");
        TimeTillAttackReset = .6f;
        AttacksPreformed = 2;
        AirAttacked = true;
        TimeTillnextAttack = .1f;
    }
    void punch(int attackNumber)
    {
        Instantiate(PunchHitBoxes[attackNumber - 1],
            transform.position + new Vector3(0, .6f, 0) + transform.forward * 1.1f, Quaternion.identity);

        PlayAnimation("Attack", attackNumber);

        AttackAgian = false;

        //this is here to fix the end lag for his attack animations
        //---------------
        if (AttacksPreformed == 1)
        {
            TimeTillAttackReset = AttackTimers[0];
        }
        if (AttacksPreformed == 2)
        {
            TimeTillAttackReset = AttackTimers[1];
        }
        if (AttacksPreformed == 1)
        {
            TimeTillAttackReset = AttackTimers[2];
        }
        //---------------
        AttacksPreformed++;
        TimeTillnextAttack = .1f;

    }
    #region Animation
    /// <summary>
    /// Call this for anytime you need to play an animation 
    /// </summary>
    /// <param name="animName"></param>
    public void PlayAnimation(string BoolName, int AttackNumber)
    {
        anim.SetInteger(BoolName, AttackNumber);
    }
    /// <summary>
    /// Call this for anytime you need to play an animation 
    /// </summary>
    /// <param name="animName"></param>
    public void PlayAnimation(string BoolName)
    {
        anim.SetBool(BoolName, true);
    }
    /// <summary>
    /// Call this for anytime you need to stop an animation
    /// </summary>
    /// <param name="BoolName"></param>
    public void StopAnimationInt(string BoolName)
    {
        anim.SetInteger(BoolName, 0);
    }
    /// <summary>
    /// Call this for anytime you need to stop an animation
    /// </summary>
    /// <param name="BoolName"></param>
    public void StopAnimationBool(string BoolName)
    {
        anim.SetBool(BoolName, false);
    }
    #endregion
}
