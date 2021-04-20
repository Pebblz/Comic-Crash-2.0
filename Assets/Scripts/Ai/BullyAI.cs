using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BullyAI : MonoBehaviour
{
    AIStates Brain;
    NavMeshAgent agent;
    public bool runningAtPlayer;
    Vector3 playerpos;
    float keepOnRunningTimer;
    [SerializeField] float RunAfterBumpTimer;
    public bool atDestination;
    [SerializeField] float BullyPushPower;
    public float KnockBackBullyPower;
    bool stumble, dead;
    float stumbleTimer;
    [SerializeField] float StumbleTime;
    [SerializeField] float MaxVelocity;
    [SerializeField] float TimeTillDead;
    private void Awake()
    {
        Brain = GetComponent<AIStates>();
        agent = GetComponent<NavMeshAgent>();
    }


    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, MaxVelocity);
        if (!dead)
        {
            if (runningAtPlayer && !stumble)
            {
                if (!atDestination)
                    agent.SetDestination(playerpos);

                if (Vector3.Distance(transform.position, playerpos) < 1.5f && !atDestination)
                {
                    keepOnRunningTimer = RunAfterBumpTimer;
                    atDestination = true;
                }

                if (keepOnRunningTimer > 0 && atDestination)
                {
                    agent.SetDestination(transform.forward * 2);
                }
                if (keepOnRunningTimer <= 0 && atDestination)
                {
                    Invoke("CallBrainAttackReset", Brain.timeBetweenAttacks);
                    atDestination = false;
                    runningAtPlayer = false;
                }
            }
            if (stumbleTimer <= 0)
            {
                Brain.StopAnimation("BumpBack");
                stumble = false;
            }
        } else
        {
            agent.SetDestination(transform.position);
        }
        stumbleTimer -= Time.deltaTime;
        keepOnRunningTimer -= Time.deltaTime;
    }

    public void Attack(Vector3 _playerpos)
    {
        agent.speed = 10;
        runningAtPlayer = true;
        playerpos = _playerpos;
    }
    void CallBrainAttackReset()
    {
        Brain.ResetAttack();
    }
    public void Stumble()
    {
        //stops agent
        agent.SetDestination(transform.position);
        stumbleTimer = StumbleTime;
        Brain.StunnedTimer(stumbleTimer);
        stumble = true;
        Brain.PlayAnimation("BumpBack");
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
    //this is for his bumping mechanic
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && runningAtPlayer)
        {
            Stumble();

            Vector3 pushDir = new Vector3(col.transform.position.x,0, col.transform.position.z) - 
                new Vector3( transform.position.x, 0 , transform.position.z);

            col.GetComponent<Player>().PushPlayer(pushDir, BullyPushPower);
        }
        if(col.tag == "Wall" && runningAtPlayer)
        {
            Stumble();

            Vector3 pushDir = new Vector3(transform.position.x, 0, transform.position.z) -new Vector3(col.transform.position.x, 0, col.transform.position.z);
            
            HitBack(pushDir, 1.5f);
        }
    }
}
