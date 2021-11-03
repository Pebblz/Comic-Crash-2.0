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
    
    void Start()
    {
        this.attack_range = this.GetComponent<SphereCollider>().radius;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void idle()
    {

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


}
