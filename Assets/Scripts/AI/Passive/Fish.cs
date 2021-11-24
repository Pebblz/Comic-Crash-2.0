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

    // Start is called before the first frame update
    private void Awake()
    {
        this.current_state = STATE.IDLE;
        max_stamina = stamina;
        init_idle_timer = idle_rest_timer;

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
        RaycastHit hit;
        Physics.Raycast(this.transform.position, this.transform.forward, out hit, 3f);
        if (hit.point != null)
        {
            random_rotate();
        }

        idle_rest_timer -= Time.deltaTime;
        if (idle_rest_timer >= 0)
            return;

        idle_rest_timer = init_idle_timer;

        if (target_pos == null || idle_rest_timer <= 0)
        {
            this.target_pos = EnemyUtils.randomVector3(-1 * x_range, x_range, 0, 0, -1 * z_range, z_range);
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
