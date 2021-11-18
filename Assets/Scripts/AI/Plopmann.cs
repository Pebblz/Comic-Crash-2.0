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
    [Tooltip("The layers the enemy makes contact with")]
    LayerMask mask;

    [SerializeField]
    [Tooltip("How fast he launch")]
    float launch_speed = 5f;


    public bool gravity = true;




    Vector3 starting_pos; // position the enemy first spawned at;

    Vector3 target_pos;

    Rigidbody body;
    bool on_wall;
    bool on_ceiling;
    bool on_ground;
    bool touched_surface;
    bool position_locked = false;

    protected override void Awake()
    {
        base.Awake();
        this.body = GetComponent<Rigidbody>();
        this.starting_pos = this.transform.position;
        this.touched_surface = true;
        this.init_charge_up = charge_up;
     


    }

    protected override void Update()
    {
        base.Update();
        if (this.position_locked)
        {
            body.constraints = RigidbodyConstraints.FreezeRotation;
            position_locked = false;
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
        this.position_locked = false;
        body.constraints = RigidbodyConstraints.FreezeRotation;


    }

    void idle()
    {
      
   
    }

    void attack()
    {

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
        }
    }



    bool on_the_ground()
    {
        Vector3 end = this.transform.position;
        end.y -= 1;
        return !Physics.Linecast(this.transform.position, end);
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().HurtPlayer(this.enemy_damage);
        }

        if(collision.contactCount > 0)
        {
            ContactPoint point = collision.contacts[0];
            body.constraints = RigidbodyConstraints.FreezePosition;
            this.position_locked = true;
            float dist = Vector3.Distance(this.transform.position, point.point) + 0.5f;
            Vector3 dir = (point.point - this.transform.position).normalized;
            Debug.DrawRay(this.transform.position, dir, Color.red, 40f);
            RaycastHit hit;
            Physics.Raycast(this.transform.position, dir, out hit);
            //Quaternion rot = Quaternion.FromToRotation(transform.forward, hit.normal);
            Quaternion rot = Quaternion.LookRotation(this.transform.position, point.normal);
            //var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //obj.transform.position = point.point;
            //this.transform.LookAt(obj.transform);
            //Destroy(obj);
            //this.transform.position = point.point;

            //body.angularVelocity = Vector3.zero;
            //body.velocity = Vector3.zero;

            //figure out where object is in relation to current transform
            var angle = Vector3.SignedAngle(this.transform.position, point.point, Vector3.up);

            //figure out where the boy be sticking to
            Debug.Log(Mathf.Rad2Deg * angle);

        } 




    }
}
