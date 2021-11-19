using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Plopmann : Enemy, IRespawnable
{

    [SerializeField]
    [Tooltip("The amount of time before the enemy launches")]
    float charge_up = 0.5f;
    float init_charge_up;

    [SerializeField]
    [Tooltip("Attack cooldown rate")]
    float attack_cooldown = 1f;
    float init_attack_cooldown;


    [SerializeField]
    [Tooltip("How fast he launch")]
    float launch_speed = 5f;
    bool attached_to_moving = false;


    public bool gravity = true;
    bool launching = false;


    Vector3 starting_pos; // position the enemy first spawned at;

    Rigidbody body;
    bool touched_surface;

    protected override void Awake()
    {
        base.Awake();
        this.body = GetComponent<Rigidbody>();
        this.starting_pos = this.transform.position;
        this.touched_surface = true;
        this.init_charge_up = charge_up;
        init_attack_cooldown = attack_cooldown;
        this.body.useGravity = gravity;
        attached_to_moving = false;
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
        this.transform.position = starting_pos;
        this.body.useGravity = true;
        this.touched_surface = true;
        this.current_state = STATE.IDLE;
        this.body.velocity = Vector3.zero;
        this.body.angularVelocity = Vector3.zero;
        this.charge_up = init_charge_up;
        body.constraints = RigidbodyConstraints.FreezeRotation;
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
        charge_up -= Time.deltaTime;
        if (charge_up > 0f)
            return;

        body.useGravity = true;
        this.transform.LookAt(target.position);
        this.body.AddForce(this.transform.forward * launch_speed, ForceMode.Impulse);
        touched_surface = false;
        charge_up = init_charge_up;
        launching = true;

    }


    void stick_to_surface()
    {
        Debug.Log("Sticking");
        if (!touched_surface)
        {
            this.touched_surface = true;
            this.body.useGravity = false;
            launching = false;
        }
    }

    bool on_the_ground()
    {

        bool on_ground = true;
        RaycastHit hit;

        Physics.Raycast(this.transform.position, Vector3.down, out hit, 0.2f, ~LayerMask.GetMask("Enemy"));
        if (hit.collider.gameObject != this.gameObject)
        {
            on_ground = false;
        }
        if (hit.point == null)
        {
            on_ground = false;
        }
        return on_ground;
    }


    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            attack_cooldown -= Time.deltaTime;
            if(attack_cooldown <= 0f)
            {
                attack_cooldown = init_attack_cooldown;
                collision.gameObject.GetComponent<PlayerHealth>().HurtPlayer(this.enemy_damage);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contactCount > 0)
        {
            ContactPoint point = collision.contacts[0];
            float dist = Vector3.Distance(this.transform.position, point.point) + 0.5f;
            Vector3 dir = (point.point - this.transform.position).normalized;
            RaycastHit hit;
            Physics.Raycast(this.transform.position, dir, out hit);

            stick_to_surface();

        }

    }

    protected override void OnTriggerEnter(Collider other)
    {
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
