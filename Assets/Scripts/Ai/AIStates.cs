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

    //for states
    [SerializeField] float sightRange, attackRange;
    [SerializeField] bool playerInSightRange, playerInAttackRange;

    [SerializeField] EnemyType enemy;
    private void Awake()
    {
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

        if (!playerInSightRange && !playerInAttackRange)
            Patroling();
        if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        if (playerInSightRange && playerInAttackRange)
            AttackPlayer();
    }
    private void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint();
        else
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalk = transform.position - walkPoint;

        if (distanceToWalk.magnitude < 1f)
            walkPointSet = false;

    }
    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX,
            transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        ////stops the agent
        //agent.SetDestination(transform.position);

        ////looks at player
        //transform.LookAt(player);

        if(!alreadyAttacked)
        {
            if(enemy == EnemyType.BullyAI)
            {
                GetComponent<BullyAI>().Attack(player.position);
            }

            alreadyAttacked = true;
            //Invoke(nameof(ResetAttack), timeBetweenAttacks);
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
