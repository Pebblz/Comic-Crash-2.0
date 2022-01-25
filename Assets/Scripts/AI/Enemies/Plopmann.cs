﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Plopmann : Enemy, IRespawnable
{

    [SerializeField]
    [Tooltip("The amount of time before the enemy launches")]
    float charge_up = 2f;
    float init_charge_up;

    [SerializeField]
    [Tooltip("Attack cooldown rate")]
    float attack_cooldown = 1f;
    float init_attack_cooldown;


    [SerializeField]
    [Tooltip("How fast he launch")]
    float launch_speed = 10f;

    [SerializeField]
    [Tooltip("Make go up more")]
    float verticle_bias = 5f;
    bool attached_to_moving = false;

    [SerializeField]
    Transform explosion_point;

    //[Tooltip("allows the toggling of gravity to happen")]
    //public bool use_gravity = true;
    //
    //private bool init_gravity = true;
    bool launching = false;

    int starting_health;

    Vector3 starting_pos; // position the enemy first spawned at;

    Rigidbody body;
    bool touched_surface;

    string attachedTo = "";

    bool Attached
    {
        get
        {
            return attachedTo != "";
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.body = GetComponent<Rigidbody>();
        this.starting_pos = this.transform.position;
        this.init_charge_up = charge_up;
        init_attack_cooldown = attack_cooldown;
        //this.body.useGravity = init_gravity;
        attached_to_moving = false;
        starting_health = this.health;
        touched_surface = true;
    }

    protected override void Update()
    {
        base.Update();

        switch (current_state){
            case STATE.IDLE:
                idle();
                break;
            case STATE.ATTACK:
                aggro();
                break;
            case STATE.STUN:
                target = null;
                break;
        }
    }

    public void reset_data()
    {
        this.health = starting_health;
        this.transform.position = starting_pos;
        //this.body.useGravity = true;
        this.current_state = STATE.IDLE;
        this.body.velocity = Vector3.zero;
        this.body.angularVelocity = Vector3.zero;
        this.charge_up = init_charge_up;
        body.constraints = RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;
        attack_cooldown = init_attack_cooldown;
        launching = false;
    }

    #region Behaviors
    // empty for now  but he'll have an animation eventually
    void idle()
    {
    }

    void aggro()
    {
        if (target == null)
            return;

        if (Attached)
        {
            attack();
        }
        else
        {
            launch();
        }
    }

    void attack()
    {
        attack_cooldown -= Time.deltaTime;
        if (attack_cooldown <= 0f)
        {
            attack_cooldown = init_attack_cooldown;
            target.gameObject.GetComponent<PlayerHealth>().HurtPlayer(this.enemy_damage);
        }
    }
    #endregion

    #region Movement

    void launch()
    {
        if (launching)
            return;

        if (!on_the_ground())
        {
            return;
        }

        charge_up -= Time.deltaTime;
        Quaternion look_rot = Quaternion.LookRotation(target.position - transform.position);
        Quaternion new_rot = Quaternion.LerpUnclamped(new Quaternion(0, look_rot.y, 0, look_rot.w), 
                                                      this.transform.rotation,
                                                      Time.deltaTime);
        this.transform.rotation = new_rot;
        if (charge_up > 0f)
            return;

        charge_up = init_charge_up;
        //body.useGravity = true;
        this.body.AddExplosionForce(launch_speed, explosion_point.transform.position, 3, verticle_bias);
        launching = true;
        touched_surface = false;

    }

    void stick_to_surface()
    {
        //this is for you pat naatz
        //thanks josh :)

        if (touched_surface)
            return;

        launching = false;
        touched_surface = true;
        //if (!this.use_gravity)
        //{
        //    this.body.useGravity = false;
        //    this.body.velocity = Vector3.zero;
        //}
    }

    bool on_the_ground()
    {
        //we use the length of the scale so we can have big plopmann's in the future
        return Physics.Raycast(transform.position, Vector3.down, transform.localScale.y + .1f);
    }
    #endregion

    #region Attach and Detatch Logic

    #region Collision Events
    private void GetShot(Collision shot)
    {
        if (Attached)
        {
            Vector3 pointOfDetatch = shot.GetContact(shot.contactCount - 1).point;
            detatch(pointOfDetatch);
        }
    }

    private void CollideWithPlayer(Collision Player)
    {
        if (Attached && Player.gameObject.name != attachedTo)
        {
            Vector3 pointOfDetatch = Player.GetContact(Player.contactCount - 1).point;
            detatch(pointOfDetatch);
        }
        else
        {
            attachTo(Player.transform);
        }
    }

    #region attachment
    private void attachTo(Transform player)
    {
        attachedTo = player.name;// + player.GetComponent<PhotonView>().ViewID.ToString();
        transform.parent = player;

        Debug.Log("now attached to " + attachedTo);
    }

    private void detatch(Vector3 pointOfDetach)
    {
        if (Attached == false)
            return;

        attachedTo = "";
        transform.parent = null;

        current_state = STATE.STUN;

        body.AddExplosionForce(launch_speed * 2, pointOfDetach, 3, verticle_bias);
    }
    #endregion

    #endregion

    #region Collisions
    private void OnCollisionStay(Collision collision)
    {
        if (this.current_state == STATE.STUN)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Shot":
                GetShot(collision);
                break;
            case "Player":
                CollideWithPlayer(collision);
                break;
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (this.current_state == STATE.STUN)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Shot":
                GetShot(collision);
                break;
            case "Player":
                CollideWithPlayer(collision);
                break;
            default:
                stick_to_surface();
                break;
        }
    }

    protected void OnCollisionExit(Collider collider)
    {//NOTE This collision is different because it returns a collider instead of a collision
        if(Attached && collider.gameObject.tag == "Player" && collider.name == attachedTo)
        {
            Vector3 pointOfDetach = collider.transform.position;
            detatch(pointOfDetach);
        }
    }
    #endregion

    #endregion

    #region Aggro
    protected override void OnTriggerEnter(Collider other)
    {
        if (this.current_state == STATE.STUN || target != null)
        {
            return;
        }

        if (other.gameObject.tag == "Player")
        {
            if (!launching)
            {
                target = other.gameObject.transform;
                this.current_state = STATE.ATTACK;
            }
        }

    }

    protected override void OnTriggerStay(Collider other)
    {
        if (this.current_state == STATE.STUN || target != null)
        {
            return;
        }

        if (other.gameObject.tag == "Player")
        {
            Debug.Log("player stayed");

            if (!launching)
            {
                target = other.gameObject.transform;
                this.current_state = STATE.ATTACK;
            }
        }
    }
    #endregion
}
