using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class WallShooter : MonoBehaviour
{
    enum SEARCH_METHOD
    {
        RANDOM_SEARCH,
        CONSTANT_ROTATION,
        FIXED,
        LEFT_TO_RIGHT,
        UP_TO_DOWN

    };

    enum ROTATION_DIRECTION { 
        CLOCKWISE,
        COUNTERCLOCKWISE
    }


    [SerializeField]
    [Tooltip("The point at which the chair rotates")]
    Transform pivot_point;

    [SerializeField]
    [Tooltip("The point where bullet are spawned from the turret")]
    Transform bullet_spawn_point;

    [SerializeField]
    [Tooltip("The search method for the AI to use")]
    SEARCH_METHOD method = SEARCH_METHOD.RANDOM_SEARCH;

    [SerializeField]
    [Tooltip("The direction to rotate in, only applies when search mode is CONSTANT_ROTATION")]
    ROTATION_DIRECTION rot_dir = ROTATION_DIRECTION.CLOCKWISE;

    [SerializeField]
    [Tooltip("Projectile the wall shoot will shoot")]
    GameObject projectile;

    [SerializeField]
    [Tooltip("Rate of fire of the gun")]
    [Range(0.2f, 2f)]
    float rof = 0.5f;
    float init_rof;

    [Header("-----------------")]
    [SerializeField]
    [Tooltip("Shows the sight cone of the wall shooter")]
    bool visible_sight_obj = true;

    [SerializeField]
    [Tooltip("The speed the mann looks around at")]
    [Range(0.5f, 5f)]
    float speed = 1f;


    [SerializeField]
    [Tooltip("The amount of time to wait before looking at a new point")]
    float rest_timeout = 1f;
    float init_timeout;

    [SerializeField]
    [Tooltip("The max value for the random x rotation")]
    [Range(1f, 180f)]
    float max_rand_x = 1f;
    [SerializeField]
    [Range(1f, 360f)]
    [Tooltip("The max value for the random y rotation")]
    float max_rand_y = 1f;

    [SerializeField]
    [Tooltip("The max value for the random x rotation")]
    [Range(-180f, -1f)]
    float min_rand_x = -1f;
    [SerializeField]
    [Range(-360f, -1f)]
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
        this.init_rof = rof;
        init_timeout = rest_timeout;
        player_sight = sight.GetComponent<PlayerSight>();
        target_rot = random_look_rot();

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        switch (this.method)
        {
            case SEARCH_METHOD.RANDOM_SEARCH:
                execute_random_search();
                break;
            case SEARCH_METHOD.CONSTANT_ROTATION:
                execute_constant_rotate();
                break;
            default:
                execute_random_search();
                break;
        }
        rest_timeout -= Time.deltaTime;
    }

    private void shoot(GameObject target)
    {
        string name = Path.Combine("PhotonPrefabs", this.projectile.name);
        var obj = PhotonNetwork.Instantiate(name, this.bullet_spawn_point.position, new Quaternion());
        obj.GetComponent<BulletScript>().target = target;
    }
    private void move_to_rotation_clamped(Vector3 target_angles)
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

    private void move_to_rotation(Quaternion target)
    {
        Vector3 current = this.transform.rotation.eulerAngles;
        Vector3 target_ang = target.eulerAngles;
        Vector3 lerped = Vector3.Lerp(current, target_ang, Time.deltaTime * speed);
        Quaternion newQ = new Quaternion();
        newQ.eulerAngles = lerped;
        this.transform.rotation = newQ;
    }


    #region SEARCH_METHODS
    private void stay_on_target()
    {
        if (this.player_sight.hasPlayer())
        {
            Vector3 player_pos = this.player_sight.getPlayer().transform.position;
            Quaternion look_rot = Quaternion.LookRotation(player_pos);
            move_to_rotation_clamped(look_rot.eulerAngles);
            //this.transform.LookAt(this.player_sight.getPlayer().transform);

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

    private Quaternion rotate()
    {
        Quaternion rot = new Quaternion();
        Vector3 angles = this.transform.rotation.eulerAngles;
        Vector3 newAngles = new Vector3();
        newAngles.x = angles.x;
        newAngles.z = 0;
        switch (this.rot_dir)
        {
            case ROTATION_DIRECTION.CLOCKWISE:
                newAngles.y = angles.y - 1f;
                break;
            case ROTATION_DIRECTION.COUNTERCLOCKWISE:
                newAngles.y = angles.y + 1f;
                break;
        }
        rot.eulerAngles = newAngles;
        return rot;
    }
    #endregion

    #region EXECUTE_SEARCH_METHODS
    
    private void execute_random_search()
    {
        if (rest_timeout <= 0f)
        {
            target_rot = random_look_rot();
            rest_timeout = init_timeout;
        }
        if (player_sight.hasPlayer())
        {
            this.execute_follow_player();
        }
        else
        {
            move_to_rotation_clamped(target_rot.eulerAngles);
        }
        rest_timeout -= Time.deltaTime;
    }

    private void execute_follow_player()
    {
        if (player_sight.hasPlayer())
        {

            if (rof <= 0f)
            {
                shoot(player_sight.getPlayer());
                rof = init_rof;
            }
            stay_on_target();
            rof -= Time.deltaTime;
        }
    }

    private void execute_constant_rotate()
    {
        if (player_sight.hasPlayer())
        {
            execute_follow_player();
        } else
        {
            switch (this.rot_dir)
            {
                case ROTATION_DIRECTION.CLOCKWISE:
                    this.transform.Rotate(Vector3.up * Time.deltaTime * speed);
                    break;
                case ROTATION_DIRECTION.COUNTERCLOCKWISE:
                    this.transform.Rotate(Vector3.down * Time.deltaTime * speed);
                    break;
            }

        }
    }
    
    #endregion
}
