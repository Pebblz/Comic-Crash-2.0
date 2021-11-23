using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChaserAI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The radius for ")]
    float sight_rad;

    [SerializeField]
    [Tooltip("Player layer mask")]
    public LayerMask player_mask;

    [SerializeField]
    [Tooltip("Speed at which the ai moves")]
    float speed = 3f;

    [SerializeField]
    [Tooltip("How close the AI has to be to the starting point before it idles")]
    [Range(0f, 1.5f)]
    float dead_zone = 0.25f;

    bool is_idle = true;
    bool at_starting_point;
    Vector3 starting_point;

    NavMeshAgent agent;


    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        this.starting_point = this.transform.position;
        this.agent = this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        bool player_in_range = Physics.CheckSphere(transform.position, sight_rad, player_mask);

        if (player_in_range)
        {
            is_idle = false;
            move_to_player();
        } else 
        {
            if(is_idle)
            {
                idle();
            }
             else if(Vector3.Distance(this.transform.position, starting_point) <= dead_zone)
            {
                at_starting_point = true;
            }
            if (at_starting_point)
            {
                is_idle = true;
                idle();
            } else
            {
                return_to_starting_point();
            }
        }
            

    }
    
    void move_to_player()
    {
        agent.SetDestination(player.transform.position);

    }

    void return_to_starting_point()
    {
        agent.SetDestination(starting_point);
        
    }

    void idle()
    {
        this.transform.rotation = new Quaternion(transform.rotation.x, 
                                                 transform.position.y + Time.deltaTime * 2, 
                                                 transform.rotation.z, transform.rotation.w);
    }
}
