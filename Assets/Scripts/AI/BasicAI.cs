using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BasicAI : MonoBehaviour
{
    [SerializeField]
    [Range(10, 500)]
    int maxHealth;

    [SerializeField]
    [Range(1, 10)]
    int Damage;

    [SerializeField]
    [Range(1, 10)]
    int HopSpeed;

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

    float jumpTimer;

    float wanderTimer;

    Vector3 wanderposition;

    bool JumpOnInteractOnce;
    [SerializeField]
    [Tooltip("The !")]
    GameObject mark;

    Rigidbody body;

    float distToGround;
    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y - .1f;
        body = GetComponent<Rigidbody>();
        wanderposition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        target = PlayerManager.instance.Player.transform;
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            if (target != PlayerManager.instance.Player)
            {
                target = PlayerManager.instance.Player.transform;
            }
        }
        else
        {
            float distance = Vector3.Distance(target.position, transform.position);

            if (distance <= LookRadius)
            {
                if (distance > agent.stoppingDistance)
                {
                    if (!JumpOnInteractOnce)
                    {
                        FoundPlayer();
                    }
                    if (jumpTimer <= 0 && IsGrounded())
                    {
                        body.isKinematic = true;
                        agent.enabled = true;
                        agent.SetDestination(target.position);
                        mark.SetActive(false);
                    }
                }
                if (distance <= agent.stoppingDistance)
                {
                    //it checks to see if the players grounded or not before attacking it 
                    if (HitTimer <= 0 && target.gameObject.GetComponent<PlayerMovement>().Grounded)
                    {
                        attackPlayer();
                        HitTimer = .3f;
                    }
                    FaceTarget();
                }
            }
            else if (distance > LookRadius && wanderTimer <= 0)
            {
                body.isKinematic = true;
                //this resets it so if it sees you again it's suprised
                JumpOnInteractOnce = false;
                if (IsGrounded() && jumpTimer <= 0)
                {
                    //this is here so if the player comes into contact with the agent
                    //it'll go back to where it was going before finding the player 
                    if (Vector3.Distance(transform.position, wanderposition) <= agent.stoppingDistance)
                    {
                        wanderposition = RandomNavSphere(transform.position, WanderRadius, -1);
                        agent.SetDestination(wanderposition);
                    }
                    else
                    {
                        wanderTimer = Random.Range(minWanderWaitTime, maxWanderWaitTime);
                        agent.SetDestination(wanderposition);
                    }
                }
            }
        }
        wanderTimer -= Time.deltaTime;
        HitTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;
    }
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
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
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    void FoundPlayer()
    {
        agent.enabled = false;
        body.isKinematic = false;
        body.velocity = new Vector3(body.velocity.x, body.velocity.y + HopSpeed, body.velocity.z);
        jumpTimer = .8f;
        mark.SetActive(true);
        transform.LookAt(new Vector3(target.position.x,transform.position.y,target.position.z));
        JumpOnInteractOnce = true;
    }
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
}