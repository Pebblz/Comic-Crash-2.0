using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideMann : Enemy
{
    // Start is called before the first frame update
    [Tooltip("The time the enemy takes to aim at a player")]
    public float look_timer = 1f;
    [Tooltip("The time before the enemy launches")]
    public float pause_timer = 0.5f;

    [SerializeField]
    [Tooltip("The layers which get exploded")]
    LayerMask layer;



    private Vector3 start_pos;
    private Vector3 target_pos;

    [SerializeField]
    [Tooltip("The amount of time to wait before an enemy moves to a new position in idle state")]
    public float rest_timeout = 0.5f;
    private float init_rest_timeout;

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



    void Start()
    {
        this.attack_range = this.GetComponent<SphereCollider>().radius;
        start_pos = this.transform.position;
        this.init_rest_timeout = rest_timeout;
    }

    // Update is called once per frame
    void Update()
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
            Vector3 new_pos =  Vector3.Lerp(this.transform.position, target_pos, Time.deltaTime);
            this.transform.position = new_pos;
        }
    }


    void attack()
    {
        look_timer -= Time.deltaTime;
        if(look_timer > 0f)
        {
            this.transform.LookAt(target);
        } else if(pause_timer >= 0f)
        {
            pause_timer -= Time.deltaTime;
        } else
        {
            explode();
        }
    }

    public void explode()
    {
      
      RaycastHit[] hits =  Physics.SphereCastAll(this.transform.position, 
                                                 attack_range, 
                                                 Vector3.up, 
                                                 attack_range, 
                                                 layer);

     foreach(var hit in hits)
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
                    }  else
                    {
                        var comps = hit.collider.gameObject.GetComponents(typeof(Component));
                        for(int i =0; i< comps.Length; i++)
                        {
                            if(comps[i] is Enemy)
                            {
                                Enemy en = (Enemy)comps[i];
                                en.damage(this.enemy_damage);

                            }
                        }
                    }
                    break;
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            this.current_state = STATE.ATTACK;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.current_state = STATE.ATTACK;
        }
    }

}
