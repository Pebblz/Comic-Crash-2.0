using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStates : MonoBehaviour
{
    NavMeshAgent agent;
    Transform player;
    [SerializeField] LayerMask whatIsGround, whatIsPlayer;

    //patrolling
    Vector3 walkPoint;
    bool walkPointSet;
    [SerializeField] float walkPointRange;

    //attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //Idle
    public float maxIdleTime, minIdleTime;
    float idleTimer;
    bool inIdle;

    //for states
    [SerializeField] float sightRange, attackRange;
    [SerializeField] bool playerInSightRange, playerInAttackRange;

    [SerializeField] EnemyType enemy;

    private Vector3 HomePoint;
    [SerializeField] float maxDistanceAwayFromHomePoint;
    private void Awake()
    {
        HomePoint = transform.position;
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = FindObjectOfType<Player>().transform;
        }
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!inIdle)
        {
            if (!playerInSightRange && !playerInAttackRange)
                Patroling();
        } else
        {
            idleTimer -= Time.deltaTime;
            if(idleTimer <= 0)
            {
                inIdle = false;
            }
        }
        if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        if (playerInSightRange && playerInAttackRange)
            AttackPlayer();
    }
    private void Patroling()
    {
        agent.speed = 5;
        if (!walkPointSet)
            SearchWalkPoint();
        else
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalk = transform.position - walkPoint;

        if (distanceToWalk.magnitude < 1f)
        {
            walkPointSet = false;
            idleTimer = Random.Range(minIdleTime, maxIdleTime);
            inIdle = true;
        }

    }
    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX,
            transform.position.y, transform.position.z + randomZ);
        if (Vector3.Distance(walkPoint, HomePoint) < maxDistanceAwayFromHomePoint)
        {
            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
                walkPointSet = true;
        } else
        {
            walkPoint = HomePoint;
            walkPointSet = true;
        }
    }
    private void ChasePlayer()
    {
        inIdle = false;
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        inIdle = false;
        if (!alreadyAttacked)
        {
            if(enemy == EnemyType.BullyAI)
            {
                GetComponent<BullyAI>().Attack(player.position);
            }
            alreadyAttacked = true;
        }
    }
    public void ResetAttack()
    {
        alreadyAttacked = false;
    }
    enum EnemyType
    {
        BullyAI,
        None
    }
}
