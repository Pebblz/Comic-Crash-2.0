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

    public int currentHealth { get; private set; }

    NavMeshAgent agent;

    [SerializeField]
    [Range(1, 20)]
    int LookRadius;

    Transform target;

    float HitTimer;

    void Start()
    {
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
            }
        }
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
}
