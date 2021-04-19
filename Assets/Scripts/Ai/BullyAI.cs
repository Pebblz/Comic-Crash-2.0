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
    [SerializeField] float KnockBackBullyPower;
    bool stumble;
    float stumbleTimer;
    [SerializeField] float StumbleTime;
    private void Awake()
    {
        Brain = GetComponent<AIStates>();
        agent = GetComponent<NavMeshAgent>();
    }


    private void FixedUpdate()
    {
        if (runningAtPlayer && !stumble)
        {
            if(!atDestination)
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
        if(stumbleTimer <= 0)
        {
            Brain.StopAnimation("BumpBack");
            stumble = false;
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
        stumble = true;
        Brain.PlayAnimation("BumpBack");
    }
    public void HitBack(Vector3 pushDir)
    {
        GetComponent<Rigidbody>().velocity = pushDir * KnockBackBullyPower;
    }
    //this is for his bumping mechanic
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            Stumble();

            Vector3 pushDir = new Vector3(col.transform.position.x,0, col.transform.position.z) - 
                new Vector3( transform.position.x, 0 , transform.position.z);

            col.GetComponent<Player>().PushPlayer(pushDir, BullyPushPower);
        }

    }
}
