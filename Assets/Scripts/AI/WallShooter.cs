using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShooter : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Projectile the wall shoot will shoot")]
    GameObject projectile;

    [Header("-----------------")]
    [SerializeField]
    [Tooltip("Shows the sight cone of the wall shooter")]
    bool visible_sight_obj = true;

    [SerializeField]
    [Tooltip("The speed the mann looks around at")]
    [Range(0.5f, 2f)]
    float speed = 1f;


    [SerializeField]
    [Tooltip("The amount of time to wait before looking at a new point")]
    float rest_timeout = 1f;
    float init_timeout;

    [SerializeField]
    [Tooltip("The max value for the random x rotation")]
    [Range(1f, 20f)]
    float max_rand_x = 1f;
    [SerializeField]
    [Range(1f, 20f)]
    [Tooltip("The max value for the random y rotation")]
    float max_rand_y = 1f;

    [SerializeField]
    [Tooltip("The max value for the random x rotation")]
    [Range(-20f, -1f)]
    float min_rand_x = -1f;
    [SerializeField]
    [Range(-20f, -1f)]
    [Tooltip("The max value for the random y rotation")]
    float min_rand_y = -1f;

    private GameObject sight;
    private Quaternion starting_rot;
    private Quaternion target_rot;

    private PlayerSight player_sight;


    private void Awake()
    {
        sight = transform.GetChild(0).gameObject;
        sight.GetComponent<MeshRenderer>().enabled = this.visible_sight_obj;
        this.starting_rot = this.transform.rotation;
        init_timeout = rest_timeout;
        player_sight = sight.GetComponent<PlayerSight>();
        target_rot = random_look_rot();

    }
    // Update is called once per frame
    void Update()
    {
        if (rest_timeout <= 0f) {
            target_rot = random_look_rot();
            rest_timeout = init_timeout;
        }

        if (player_sight.hasPlayer())
        {
            stay_on_target();
        } else
        {
            move_to_rotation(target_rot.eulerAngles);
        }
        rest_timeout -= Time.deltaTime;
    }
    
    private void move_to_rotation(Vector3 target_angles)
    {
        Vector3 current = this.transform.rotation.eulerAngles;
        Vector3 target_clamped = new Vector3(Mathf.Clamp(target_angles.x,
                                                        starting_rot.eulerAngles.x + this.min_rand_x,
                                                        starting_rot.eulerAngles.x + max_rand_x),
                                             Mathf.Clamp(target_angles.y, 
                                                        starting_rot.eulerAngles.y + this.min_rand_y,
                                                        starting_rot.eulerAngles.y + this.max_rand_y),
                                             0f);

        Vector3 poo = Vector3.Lerp(current, target_clamped, Time.deltaTime * speed);
        Quaternion new_boy = new Quaternion();
        new_boy.eulerAngles = poo;
        this.transform.rotation = new_boy;
    }

    private void stay_on_target()
    {
        if (this.player_sight.hasPlayer())
        {
            this.transform.LookAt(this.player_sight.getPlayer().transform, Vector3.down);
    
        }
        
    }

    private Quaternion random_look_rot()
    {
        float rand_x = Random.Range(this.min_rand_x, this.max_rand_x);
        float rand_y = Random.Range(this.min_rand_y, this.max_rand_y);
        Vector3 startAngles = this.starting_rot.eulerAngles;
        Vector3 randomAngles = new Vector3(rand_x, rand_y, 0);
        Quaternion target = new Quaternion();
        target.eulerAngles = (startAngles + randomAngles);
        return target;
    }
}
