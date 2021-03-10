using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BasicAI : MonoBehaviour
{
    [SerializeField]
    [Range(10, 500)]
    int maxHealth ;

    [SerializeField]
    [Range(1, 10)]
    int Damage;


    [SerializeField]
    [Range(.1f, 5f)]
    float maxWanderWaitTime;

    [SerializeField]
    [Range(.1f, 10f)]
    float minWanderWaitTime;

    public int currentHealth { get; private set; }

    NavMeshAgent agent;

    [SerializeField]
    [Range(1, 20)]
    int LookRadius;

    [SerializeField]
    [Range(1, 20)]
    int WanderRadius;

    Transform target;

    float HitTimer;

    float wanderTimer;

    Vector3 wanderposition;
    void Start()
    {
        //this is here so you can start wandering 
        wanderposition = transform.position;

        agent = GetComponent<NavMeshAgent>();
        target = PlayerManager.instance.Player.transform;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if(target == null)
        {
            if(target != PlayerManager.instance.Player)
            {
                target = PlayerManager.instance.Player.transform;
            }
        }else
        {
            

            float distance = Vector3.Distance(target.position, transform.position);

            if(distance <= LookRadius)
            {
                if (distance > agent.stoppingDistance)
                {
                    agent.SetDestination(target.position);
                }
                if(distance <= agent.stoppingDistance)
                {
                    if (HitTimer <= 0)
                    {
                        attackPlayer();
                        HitTimer = .3f;
                    }
                    FaceTarget();
                }
            } else if(distance > LookRadius && wanderTimer <= 0)
            {
                //this is here so if the player comes into contact with the agent
                //it'll go back to where it was going before finding the player 
                if (Vector3.Distance(transform.position, wanderposition) <= agent.stoppingDistance)
                {
                    wanderposition = RandomNavSphere(transform.position, WanderRadius, -1);
                    agent.SetDestination(wanderposition);
                } else
                {
                    wanderTimer = Random.Range(minWanderWaitTime, maxWanderWaitTime);
                    agent.SetDestination(wanderposition);
                }
                
            }
        }
        wanderTimer -= Time.deltaTime;
        HitTimer -= Time.deltaTime;
    }
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    public void AIHit(int HealthTaken)
    {
        currentHealth -= HealthTaken;
    }
    public void AIHeal(int HealthGiven)
    {
        currentHealth += HealthGiven;
    }
    void attackPlayer()
    {
        target.gameObject.GetComponent<PlayerHealth>().HurtPlayer(Damage);
    }
    void die()
    {
        //play Death animation

        Destroy(gameObject);
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
