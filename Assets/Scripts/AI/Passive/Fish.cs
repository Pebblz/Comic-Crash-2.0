using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    enum STATE
    {
        IDLE,
        CAUGHT,
        ON_THE_HOOK,
        RESPAWN
    }

    public float x_range;
    public float z_range;
    public float y_range;

    public int stamina;
    private int max_stamina;
    public int weight;
    public int stamina_recover_rate;
    private STATE current_state;

    public float idle_rest_timer;
    private float init_idle_timer;

    public float swim_speed;

    public Vector3 catch_position;


    private float time_on_hook = 0f;
    private Vector3 target_pos;
    private Vector3 starting_pos;
    private bool looking_at_target = false;
    // Start is called before the first frame update
    private void Awake()
    {
        this.current_state = STATE.IDLE;
        max_stamina = stamina;
        init_idle_timer = idle_rest_timer;
        starting_pos = this.transform.position;
        this.target_pos = starting_pos + EnemyUtils.randomVector3(-1 * x_range, x_range, 0, 0, -1 * z_range, z_range);
        looking_at_target = false;


}


// Update is called once per frame
void Update()
    {
        switch (current_state)
        {
            case STATE.IDLE:
                swim_idle();
                break;

        }
    }

    private void swim_idle()
    {
        idle_rest_timer -= Time.deltaTime;
        if (target_pos == null || idle_rest_timer <= 0 )
        {
            idle_rest_timer = init_idle_timer;
            this.target_pos = starting_pos + EnemyUtils.randomVector3(-1 * x_range, x_range, 0, 0, -1 * z_range, z_range);
            looking_at_target = false;
        } else
        {
            Vector3 new_pos = Vector3.Lerp(this.transform.position, target_pos, Time.deltaTime/2);

            if (!looking_at_target)
            {
                transform.LookAt(new_pos);
                looking_at_target = true;
            }
            this.transform.position = new_pos;
        }
     
    



    }

    private void random_rotate()
    {
        Quaternion t = Random.rotation;
        Vector3 what_i_actually_care_about = new Vector3(0, t.y, 0);
        Quaternion new_boi = new Quaternion();
        new_boi.eulerAngles = what_i_actually_care_about;
        this.transform.rotation = new_boi;
    }


    

}
