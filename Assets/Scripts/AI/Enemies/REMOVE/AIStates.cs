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
    public bool dead;
    public bool stunned;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        HomePoint = transform.position;
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead && !stunned)
        {
            if (player == null)
            {
                player = FindObjectOfType<Player>().transform;
            }
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!inIdle)
            {
                if (!playerInSightRange && !playerInAttackRange)
                    Patroling();
            }
            else
            {
                StopAnimation("IsWalking");
                StopAnimation("IsCharging");
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0)
                {
                    inIdle = false;
                }
            }
            if (playerInSightRange && !playerInAttackRange)
                ChasePlayer();
            if (playerInSightRange && playerInAttackRange)
                AttackPlayer();
        }
    }
    private void Patroling()
    {
        PlayAnimation("IsWalking");
        StopAnimation("IsCharging");
        agent.speed = 3;
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
        if (Physics.Raycast(player.position, -transform.up, 4f, whatIsGround))
        {
            StopAnimation("IsWalking");
            PlayAnimation("IsCharging");
            inIdle = false;
            agent.SetDestination(player.position);
        } else
        {
            Patroling();
        }
    }
    private void AttackPlayer()
    {
        if (Physics.Raycast(player.position, -transform.up, 4f, whatIsGround))
        {
            inIdle = false;
            if (!alreadyAttacked)
            {
                if (enemy == EnemyType.BullyAI)
                {
                    GetComponent<BullyAI>().Attack(player.position);
                }
                if(enemy == EnemyType.BasicEnemy)
                {
                    GetComponent<BasicEnemyAI>().Attack();
                }
                alreadyAttacked = true;
            }
        } else
        {
            Patroling();
        }
    }
    public void ResetAttack()
    {
        alreadyAttacked = false;
    }
    public void StunnedTimer(float Timer)
    {
        stunned = true;
        Invoke("UnStun", Timer);
    }
    void UnStun()
    {
        stunned = false;
    }
    enum EnemyType
    {
        BullyAI,
        BasicEnemy,
        None
    }



    #region Animation
    /// <summary>
    /// Call this for anytime you need to play an animation 
    /// </summary>
    /// <param name="animName"></param>
    public void PlayAnimation(string BoolName)
    {
        anim.SetBool(BoolName, true);
    }
    /// <summary>
    /// Call this for anytime you need to stop an animation
    /// </summary>
    /// <param name="BoolName"></param>
    public void StopAnimation(string BoolName)
    {
        anim.SetBool(BoolName, false);
    }
    #endregion
}
