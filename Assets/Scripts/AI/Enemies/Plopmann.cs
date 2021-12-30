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

    [Tooltip("allows the toggling of gravity to happen")]
    public bool use_gravity = true;

    private bool init_gravity = true;
    bool launching = false;

    int starting_health;

    Vector3 starting_pos; // position the enemy first spawned at;

    Rigidbody body;
    bool touched_surface;

    protected override void Awake()
    {
        base.Awake();
        this.body = GetComponent<Rigidbody>();
        this.starting_pos = this.transform.position;
        this.init_charge_up = charge_up;
        init_attack_cooldown = attack_cooldown;
        this.body.useGravity = init_gravity;
        attached_to_moving = false;
        starting_health = this.health;
        touched_surface = true;
    }

    protected override void Update()
    {
        base.Update();

        if (this.current_state == STATE.IDLE)
        {
            idle();
        }

        if (this.current_state == STATE.ATTACK)
        {
            launch();
        }
       
    }

    public void reset_data()
    {
        this.health = starting_health;
        this.transform.position = starting_pos;
        this.body.useGravity = true;
        this.current_state = STATE.IDLE;
        this.body.velocity = Vector3.zero;
        this.body.angularVelocity = Vector3.zero;
        this.charge_up = init_charge_up;
        body.constraints = RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;
        attack_cooldown = init_attack_cooldown;
        launching = false;


    }

    // empty for now  but he'll have an animation eventually
    void idle()
    { }


    void launch()
    {
        if (launching)
            return;
        if (!on_the_ground())
            return;
        charge_up -= Time.deltaTime;
        Quaternion look_rot = Quaternion.LookRotation(target.position - transform.position);
        Quaternion new_rot = Quaternion.LerpUnclamped(new Quaternion(0, look_rot.y, 0, look_rot.w), 
                                                      this.transform.rotation,
                                                      Time.deltaTime);
        this.transform.rotation = new_rot;
        if (charge_up > 0f)
            return;

        charge_up = init_charge_up;
        body.useGravity = true;
        this.body.AddExplosionForce(launch_speed, explosion_point.transform.position, 3, verticle_bias);
        launching = true;
        touched_surface = false;

    }


    void stick_to_surface()
    {
        //this is for you pat naatz
        if (touched_surface)
            return;

        launching = false;
        touched_surface = true;
        if (!this.use_gravity)
        {
            this.body.useGravity = false;
            this.body.velocity = Vector3.zero;
        }

    }

    bool on_the_ground()
    {
        RaycastHit hit;
        //Debug.DrawRay(this.transform.position, Vector3.down * 0.4f, Color.green, 4f);
        return Physics.Raycast(this.transform.position, Vector3.down, out hit, 0.4f, ~LayerMask.GetMask("Enemy"));
    }


    private void OnCollisionStay(Collision collision)
    {
        if (this.current_state == STATE.STUN)
        {
            return;
        }
        if (collision.gameObject.tag == "Player")
        {
            attack_cooldown -= Time.deltaTime;
            if (attack_cooldown <= 0f)
            {
                attack_cooldown = init_attack_cooldown;
                collision.gameObject.GetComponent<PlayerHealth>().HurtPlayer(this.enemy_damage);
            }
        }
        if (collision.gameObject.tag != "Shot")
        {
            stick_to_surface();
        }

    }

    protected override void OnCollisionEnter(Collision collision)
    {

        base.OnCollisionEnter(collision);

        if (collision.contactCount > 0)
        {
            /*
            ContactPoint point = collision.contacts[0];
            float dist = Vector3.Distance(this.transform.position, point.point) + 0.5f;
            Vector3 dir = (point.point - this.transform.position).normalized;
            RaycastHit hit;
            Physics.Raycast(this.transform.position, dir, out hit);
            */

        }
        if (collision.gameObject.tag != "Shot")
        {
            stick_to_surface();
        }

    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (this.current_state == STATE.STUN)
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
        if (this.current_state == STATE.STUN)
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
}
