using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyAI : MonoBehaviour
{
    AIStates Brain;
    NavMeshAgent agent;
    public bool runningAtPlayer;
    GameObject player;
    Vector3 playerpos;
    public bool atDestination;
    bool  dead;
    float AttackTimer;
    [SerializeField] float MaxVelocity;
    [SerializeField] float TimeTillDead;
    [SerializeField] int damageDealt;
    [SerializeField] float MaxAttackTimer;
    private void Awake()
    {
        Brain = GetComponent<AIStates>();
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerMovement>().gameObject;
    }
    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, MaxVelocity);
        if (!dead)
        {
            if(player == null)
                player = FindObjectOfType<PlayerMovement>().gameObject;
            if (runningAtPlayer)
            {
                if (!atDestination)
                    agent.SetDestination(playerpos);

                if (Vector3.Distance(transform.position, playerpos) < 1.5f && !atDestination)
                {
                    atDestination = true;
                }

                if (atDestination)
                {
                    agent.SetDestination(transform.forward * 2);
                }
                if (atDestination && AttackTimer < 0)
                {
                    Invoke("CallBrainAttackReset", Brain.timeBetweenAttacks);
                    atDestination = false;
                    runningAtPlayer = false;
                }
            }
        }
        else
        {
            agent.SetDestination(transform.position);
        }
    }

    public void Attack()
    {
        AttackTimer = MaxAttackTimer;
        player.GetComponent<PlayerHealth>().HurtPlayer(damageDealt);
    }
    void CallBrainAttackReset()
    {
        Brain.ResetAttack();
    }
    public void HitBack(Vector3 pushDir, float _KnockBackBullyPower)
    {
        GetComponent<Rigidbody>().velocity = pushDir * _KnockBackBullyPower;
    }
    public void StartDeath()
    {
        dead = true;
        Brain.dead = true;
        Brain.PlayAnimation("IsDead");
        Invoke("Destroy", TimeTillDead);
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
