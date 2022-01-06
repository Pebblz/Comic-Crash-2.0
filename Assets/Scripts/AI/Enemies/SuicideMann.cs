using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideMann : Enemy, IRespawnable
{
    // Start is called before the first frame update
    [Tooltip("The time the enemy takes to aim at a player")]
    public float look_timeout = 1f;
    [Tooltip("The time before the enemy launches")]
    public float pause_timeout = 0.5f;
    
    [SerializeField]
    [Tooltip("The amount of time to wait before an enemy moves to a new position in idle state")]
    public float rest_timeout = 0.5f;
    
    [SerializeField]
    [Tooltip("amount of time for the enemy to detonate")]
    float detonation_time = 2.5f;

    [SerializeField]
    [Tooltip("The layers which get exploded")]
    LayerMask layer;

    [SerializeField]
    [Tooltip("Speed at which the enemy chases player")]
    float zoom_speed = 5f;

    [SerializeField]
    [Range(1, 10)]
    float max_x;
    [SerializeField]
    [Range(1, 10)]
    float max_y;
    [SerializeField]
    [Range(1, 10)]
    float max_z;
    [SerializeField]
    [Range(-10, -1)]
    float min_x;
    [SerializeField]
    [Range(-10, -1)]
    float min_y;
    [SerializeField]
    [Range(-10, -1)]
    float min_z;

    private Vector3 start_pos;
    private Vector3 target_pos;
    private float init_look_timeout;
    private float init_rest_timeout;
    private float init_pause_timeout;
    private float init_det_timeout;
    private int init_health;

    Rigidbody body;
    bool exploding = false;
    bool zoomed = false;
    [SerializeField]
    Animator animator;
    public void reset_data()
    {
        exploding = false;
        zoomed = false;
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = false;
        this.transform.position = start_pos;
        this.target_pos = start_pos;
        this.current_state = STATE.IDLE;
        this.health = init_health;
        this.rest_timeout = init_rest_timeout;
        this.look_timeout = init_look_timeout;
        this.pause_timeout = init_pause_timeout;
        this.detonation_time = init_det_timeout;
    }

    protected override void Awake()
    {
        base.Awake();
        this.target_pos = this.transform.position;
        this.init_look_timeout = look_timeout;
        this.init_pause_timeout = pause_timeout;
        this.init_rest_timeout = rest_timeout;
        this.init_det_timeout = detonation_time;
        this.init_health = this.health;
        start_pos = this.transform.position;
        this.attack_range = this.GetComponent<SphereCollider>().radius;
        body = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        switch (this.current_state)
        {
            case STATE.IDLE:
                idle();
                break;
            case STATE.ATTACK:
                attack();
                break;
        }
    }

    void idle()
    {
        rest_timeout -= Time.deltaTime;

        if (target_pos == null || rest_timeout <= 0f)
        {
            target_pos = start_pos + EnemyUtils.randomVector3(min_x, max_x, min_y, max_y, min_z, max_z);
            rest_timeout = init_rest_timeout;

        }
        else
        {
            Vector3 new_pos = Vector3.Lerp(this.transform.position, target_pos, Time.deltaTime);
            this.transform.position = new_pos;
        }
    }


    void attack()
    {
        look_timeout -= Time.deltaTime;
        if (look_timeout > 0f)
        {
            this.transform.LookAt(target);
            animator.SetBool("anticipation",true);
        }
        else if (pause_timeout >= 0f)
        {
            pause_timeout -= Time.deltaTime;
        }
        else
        {
            if (detonation_time <= 0f)
            {
                explode();
            }
            else
            {
                if (!zoomed)
                {
                    animator.SetBool("zoom", true);
                    body.AddForce(this.transform.forward * zoom_speed, ForceMode.Impulse);
                    zoomed = true;
                }
                detonation_time -= Time.deltaTime;
            }
        }
    }


    public void explode()
    {
        if (exploding == true)
        {
            return;
        }
        exploding = true;

        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position,
                                                   attack_range,
                                                   Vector3.up,
                                                   attack_range,
                                                   layer);

        foreach (var hit in hits)
        {
            string tag = hit.collider.gameObject.tag;
            switch (tag)
            {
                case "Player":
                    hit.collider.gameObject.GetComponent<PlayerHealth>().HurtPlayer(this.enemy_damage);
                    break;
                case "Enemy":
                    var mann = hit.collider.gameObject.GetComponent<SuicideMann>();
                    if (mann != null)
                    {
                        mann.explode();
                    }
                    else
                    {
                        var comps = hit.collider.gameObject.GetComponents(typeof(Component));
                        for (int i = 0; i < comps.Length; i++)
                        {
                            if (comps[i] is Enemy)
                            {
                                Enemy en = (Enemy)comps[i];
                                en.damage(this.enemy_damage);

                            }
                        }
                    }
                    break;
            }
        }

        die();

    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (this.current_state == STATE.STUN)
        {
            return;
        }
        if (other.gameObject.tag == "Player")
        {
            this.current_state = STATE.ATTACK;
            this.target = other.gameObject.transform;
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
            this.current_state = STATE.ATTACK;
            this.target = other.gameObject.transform;
            ;
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.tag == "Shot")
        {
            body.useGravity = true;
        }
        else
        {
            this.explode();
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.target = other.gameObject.transform;
        }
    }

}
