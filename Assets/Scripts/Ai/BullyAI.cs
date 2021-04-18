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
    public float keepOnRunningTimer;
    public bool atDestination;
    [SerializeField] float BullyPushPower;
    private void Awake()
    {
        Brain = GetComponent<AIStates>();
        agent = GetComponent<NavMeshAgent>();
    }


    private void FixedUpdate()
    {
        if (runningAtPlayer)
        {
            if(!atDestination)
                agent.SetDestination(playerpos);

            if (Vector3.Distance(transform.position, playerpos) < 1.5f && !atDestination)
            {
                keepOnRunningTimer = 2;
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
        keepOnRunningTimer -= Time.deltaTime;
    }

    public void Attack(Vector3 _playerpos)
    {
        runningAtPlayer = true;
        playerpos = _playerpos;
    }
    void CallBrainAttackReset()
    {
        Brain.ResetAttack();
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            Vector3 pushDir = new Vector3(col.transform.position.x,0, col.transform.position.z) - 
                new Vector3( transform.position.x, 0 , transform.position.z);

            col.GetComponent<Player>().PushPlayer(pushDir, BullyPushPower);
        }
    }
}
