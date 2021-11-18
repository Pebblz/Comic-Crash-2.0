using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plopmann : Enemy, IRespawnable
{

    [SerializeField]
    [Tooltip("The amount of time before the enemy launches")]
    float charge_up = 0.5f;
    float init_charge_up;

    [SerializeField]
    [Tooltip("The layers the enemy makes contact with")]
    LayerMask mask;

    [SerializeField]
    [Tooltip("How fast he launch")]
    float launch_speed = 5f;

    [SerializeField]
    [Tooltip("Random Horizontal range")]
    float hori_range = 2f;

    [SerializeField]
    [Tooltip("Random Vertical range")]
    float vert_range = 2f;

    [SerializeField]
    [Tooltip("Timeout before he finds a new position in idle state")]
    float idle_timeout;
    float init_idle_timeout;

    public bool gravity = true;




    Vector3 starting_pos; // position the enemy first spawned at;
    Vector3 current_pos; // position the enemy is currently sticking to

    Vector3 target_pos;

    Rigidbody body;
    bool on_wall;
    bool on_ceiling;
    bool on_ground;
    bool touched_surface;

    protected override void Awake()
    {
        base.Awake();
        this.body = GetComponent<Rigidbody>();
        this.starting_pos = this.transform.position;
        this.touched_surface = true;
        this.init_charge_up = charge_up;
        this.init_idle_timeout = idle_timeout;


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
        this.idle_timeout = init_idle_timeout;
        
    }

    void idle()
    {
        idle_timeout -= Time.deltaTime;
        if(idle_timeout <= 0f)
        {
            idle_timeout = init_idle_timeout;
            if (on_the_ground())
            {
                Vector3 new_pos = EnemyUtils.randomVector3(this.hori_range, 0f, this.vert_range);
            }
            else if (this.on_ceiling)
            {

            }
            else if (this.on_wall)
            {

            }
        }
   
    }

    void launch()
    {
        charge_up -= Time.deltaTime;
        if (charge_up > 0f)
            return; 

        body.useGravity = true;
        this.transform.LookAt(target.position);
        this.body.AddForce(this.transform.forward * launch_speed, ForceMode.Impulse);
        touched_surface = false;
        on_wall = false;
        on_ceiling = false;
    }


    void stick_to_surface()
    {
        if (!touched_surface)
        {
            this.touched_surface = true;
            this.body.useGravity = false;
            this.current_pos = this.transform.position;
        }
    }



    bool on_the_ground()
    {
        Vector3 end = this.transform.position;
        end.y -= 1;
        return Physics.Linecast(this.transform.position, end);
    }

    bool on_the_ceiling()
    {
        Vector3 end = this.transform.position;
        end.y += 1;
        return Physics.Linecast(this.transform.position, end);
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().HurtPlayer(this.enemy_damage);
        }

        foreach (ContactPoint point in collision.contacts)
        {
            float dist = Vector3.Distance(this.transform.position, point.point) + 0.5f;

            Debug.DrawRay(this.transform.position, point.normal, Color.red, 4f);
            RaycastHit hit; 
            Physics.Raycast(this.transform.position, point.normal, out hit);
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, point.normal);
            this.transform.position = point.point;
            transform.rotation = rot;
            body.angularVelocity = Vector3.zero;
            body.velocity = Vector3.zero;

            //figure out where object is in relation to current transform
            var angle = Vector3.SignedAngle(this.transform.position, point.point, Vector3.up);

            //figure out where the boy be sticking to
            Debug.Log(Mathf.Rad2Deg * angle);
            

        }
    }
}
