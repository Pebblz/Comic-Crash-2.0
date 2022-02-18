using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SpiderGoop : MonoBehaviour
{
    #region Vars
    [SerializeField]
    private float rotSpeed, idleMoveSpeed, chaseSpeed, chargeSpeed,
        timeTillIdle, playerSeenRange, chargeCooldown, chargeDuration, distAwayToCharge = 8;

    [SerializeField]
    private int damage = 1;

    [SerializeField, Tooltip("The different idle states")]
    WaysToIdle idleWays = WaysToIdle.StandAtPoint;

    [SerializeField, Tooltip("Should the enemy go back to starttign rotation when going back to idle")]
    bool realignWithStartingRot;

    GameObject chasedPlayer;

    Vector3 lastSeenPlayerPos;

    Animator anim;

    List<GameObject> detectedPlayers = new List<GameObject>();

    EnemyDetection Detection;

    Vector3 startingPoint;

    Rigidbody rb;

    [SerializeField]
    List<GameObject> waypoints = new List<GameObject>();

    float timerTillIdle;

    Quaternion startingRot;

    int currentWaypointIndex;

    float chargeCooldownTimer, chargeDurationTimer;

    bool jumping, charging;
    #endregion

    private void Start()
    {
        anim = GetComponent<Animator>();
        Detection = GetComponent<EnemyDetection>();
        startingPoint = transform.position;
        startingRot = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!charging)
        {
            if (Detection.IsPlayerInSight()
                || detectedPlayers.Count > 0
                || lastSeenPlayerPos != Vector3.zero)
                ChasePlayer();
            else
            {
                if (idleWays == WaysToIdle.StandAtPoint)
                {
                    if (Vector3.Distance(transform.position, startingPoint) > 1.5f)
                    {
                        if (timerTillIdle > 0)
                        {
                            anim.SetBool("Idle", true);
                            timerTillIdle -= Time.deltaTime;
                        }
                        else
                        {
                            anim.SetBool("Idle", false);
                            ReturnToStartingPoint();
                        }
                    }
                    else
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, startingRot, 3 * Time.deltaTime);
                    }
                }
                if (idleWays == WaysToIdle.WayPoint)
                {
                    if (timerTillIdle > 0)
                    {
                        anim.SetBool("Idle", true);
                        timerTillIdle -= Time.deltaTime;
                    }
                    else
                    {
                        anim.SetBool("Idle", false);
                        WaypointMovement();
                    }
                }
            }
            chargeCooldownTimer -= Time.deltaTime;
        }
        else
        {
            ChargeAtPlayer();
            chargeDurationTimer -= Time.deltaTime;
            if (chargeDurationTimer <= 0)
            {
                ResetCharge();
            }
        }
    }

    private void ChasePlayer()
    {
        if (detectedPlayers.Count == 0)
        {
            detectedPlayers = Detection.GetPlayersInSight();
            if (lastSeenPlayerPos != Vector3.zero)
            {
                //this is here so the enemy wont have a point to go to that's in the air
                Vector3 ChasedPlayerpos = new Vector3(lastSeenPlayerPos.x, transform.position.y, lastSeenPlayerPos.z);

                //rotation
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                  Quaternion.LookRotation(ChasedPlayerpos - transform.position),
                                  rotSpeed * Time.deltaTime);
                //the direction of the last seen pos 
                Vector3 dir = (ChasedPlayerpos - transform.position).normalized;
                //this sees if the enemies looking at the last seen pos
                float dot = Vector3.Dot(dir, transform.forward);
                if (!ShouldJump(lastSeenPlayerPos))
                {
                    //if dot's 1 then the enemies looking at the vector3 pos if -1 enemies looking the wrong way
                    if (dot > .9f && !jumping)
                    {
                        rb.velocity = (transform.forward * chaseSpeed) + new Vector3(0, rb.velocity.y, 0);
                    }
                }
                else if (!jumping)
                {

                    anim.SetBool("Jump", true);
                    anim.SetBool("OnGround", false);
                }
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !anim.GetAnimatorTransitionInfo(0).IsName("Jump -> Walk") && !jumping)
                {
                    jumping = true;
                    rb.velocity += (transform.forward * 6) + new Vector3(0, GetJumpHeight(lastSeenPlayerPos.y));
                }
                //if the enemies at the last seen pos
                if (Vector3.Distance(transform.position, ChasedPlayerpos) < .3f || lastSeenPlayerPos.y > transform.position.y + 8)
                    lastSeenPlayerPos = Vector3.zero;
            }
        }
        else
        {
            //gets a player to chase
            if (chasedPlayer == null)
            {
                chasedPlayer = Detection.FindClosestPlayer(detectedPlayers);
            }
            else
            {
                Vector3 ChasedPlayersPos = new Vector3(chasedPlayer.transform.position.x, transform.position.y, chasedPlayer.transform.position.z);
                if (Vector3.Distance(transform.position, ChasedPlayersPos) > playerSeenRange)
                {
                    lastSeenPlayerPos = chasedPlayer.transform.position;
                    //move to last seen player pos then have a countdown till going back to idle
                    timerTillIdle = timeTillIdle;
                    //then do this
                    detectedPlayers.Clear();
                    chasedPlayer = null;
                }
                else
                {
                    //Rotates slowly towards player
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(ChasedPlayersPos - transform.position),
                                                      rotSpeed * Time.deltaTime);

                    if (!ShouldJump(chasedPlayer.transform.position))
                    {

                        //Moves enemy towards player
                        if (Vector3.Distance(transform.position, chasedPlayer.transform.position) > 2.5f && !jumping && !charging)
                            rb.velocity = (transform.forward * chaseSpeed) + new Vector3(0, rb.velocity.y, 0);
                        //charges at player
                        if (Vector3.Distance(transform.position, chasedPlayer.transform.position) <= distAwayToCharge && !jumping && chargeCooldownTimer <= 0)
                            charging = true;
                    }
                    else if (!jumping)
                    {
                        anim.SetBool("Jump", true);
                        anim.SetBool("OnGround", false);
                    }
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !anim.GetAnimatorTransitionInfo(0).IsName("Jump -> Walk") && !jumping)
                    {
                        jumping = true;
                        rb.velocity += (transform.forward * 6) + new Vector3(0, GetJumpHeight(chasedPlayer.transform.position.y));
                    }
                }
            }
        }
        if (rb.velocity.y > -.2f && rb.velocity.y < .2f && jumping)
        {
            jumping = false;
            anim.SetBool("Jump", false);
            anim.SetBool("OnGround", true);
        }
    }

    void ReturnToStartingPoint()
    {
        Vector3 goToPos = new Vector3(startingPoint.x, transform.position.y, startingPoint.z);

        //Rotates slowly towards startPos
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                              Quaternion.LookRotation(goToPos - transform.position),
                                              rotSpeed * Time.deltaTime);
        //Moves enemy towards startPos
        rb.velocity = transform.forward * idleMoveSpeed;
    }

    void WaypointMovement()
    {
        if (waypoints.Count > 0)
        {
            //this is here just so we don't have the enemy fly
            Vector3 currentWaypointPos = new Vector3(waypoints[currentWaypointIndex].transform.position.x, transform.position.y, waypoints[currentWaypointIndex].transform.position.z);

            if (Vector3.Distance(transform.position, currentWaypointPos) > .2f)
            {

                //Rotates slowly towards waypoint
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(currentWaypointPos - transform.position),
                                                      rotSpeed * Time.deltaTime);
                //Moves enemy towards waypoint
                rb.velocity = (transform.forward + new Vector3(0, rb.velocity.y, 0)) * idleMoveSpeed;

            }
            else
            {
                if (currentWaypointIndex < waypoints.Count)
                    currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Count)
                    currentWaypointIndex = 0;
            }
        }
        else
        {
            //just in case
            idleWays = WaysToIdle.StandAtPoint;
        }
    }

    void ChargeAtPlayer()
    {
        anim.SetBool("Charge", true);
        rb.velocity = (transform.forward + new Vector3(0, rb.velocity.y, 0)) * chargeSpeed;
    }
    
    void DamagePlayer(GameObject AttackedPlayer)
    {
        PlayerHealth Player = AttackedPlayer.GetComponent<PlayerHealth>();
        if (Player != null)
        {
            Player.HurtPlayer(damage);
        }
    }

    void ResetCharge()
    {
        anim.SetBool("Charge", false);
        chargeCooldownTimer = chargeCooldown;
        chargeDurationTimer = chargeDuration;
        charging = false;
    }

    bool ShouldJump(Vector3 HuntedTarget)
    {
        if (HuntedTarget.y > transform.position.y)
        {
            Vector3 start = this.gameObject.transform.position + new Vector3(0, .5f, 0);
            RaycastHit hit;
            bool TopHit = false;
            for (float bottom = -1; bottom <= 1; bottom += .2f)
            {
                //if the bottom one hits then that means he's colliding with a object
                if (Physics.Raycast(start + new Vector3(bottom, 0, 0), transform.TransformDirection(Vector3.forward), out hit, 4f))
                {
                    if (hit.collider.gameObject.tag != "Player" && !hit.collider.isTrigger)
                    {
                        for (float top = -1; top <= 1; top += .2f)
                        {
                            //if he collides with an object up top then that means he shouldn't jump 
                            if (Physics.Raycast(start + new Vector3(top, 4f, 0), transform.TransformDirection(Vector3.forward), out hit, 4f))
                            {
                                TopHit = true;
                            }
                        }
                        //if the top hit he shouldn't jump
                        if (TopHit)
                            return false;
                        else
                        {
                            return true;
                        }
                    }

                }
            }
        }
        else
        {
            return false;
        }
        return false;
    }

    float GetJumpHeight(float chasedTargetY)
    {
        //this function's here so the Enemy doesn't jump his highest on small ledges
        float temp = chasedTargetY - transform.position.y;

        if (temp <= 2.3f)
            temp += 8;
        else
            temp += 12;

        return temp;
    }

    private void OnTriggerEnter(Collider col)
    {
        if(charging && col.gameObject.tag == "Player")
        {
            //also knockback player
            DamagePlayer(col.gameObject);
        }
        if (charging && col.gameObject.tag != "Player")
        {
            //make here for stunning the enemy when hitting a wall
            chargeDurationTimer = 0;
        }

    }

    enum WaysToIdle
    {
        StandAtPoint,
        WayPoint
    }

    #region Joshes Old code
    //[SerializeField]
    //[Tooltip("Distance Enemy has to be from a point to start targeting the next")]
    //float point_dist = 0.3f;
    //[SerializeField]
    //PatrolPath path;
    //private NavMeshAgent agent;
    //Transform patrol_point;
    //private void Awake()
    //{
    //    agent = this.GetComponent<NavMeshAgent>();
    //    patrol_point = path.getNextPoint();
    //}


    //// Update is called once per frame
    //void Update()
    //{
    //    idle();
    //}

    //void idle()
    //{
    //    if (patrol_point == null)
    //        patrol_point = path.getNextPoint();
    //    if (Vector3.Distance(this.transform.position, patrol_point.position) <= point_dist)
    //        patrol_point = path.getNextPoint();

    //    Quaternion lookRot = Quaternion.LookRotation(patrol_point.position - transform.position);
    //    Quaternion rot = Quaternion.Lerp(this.transform.rotation, lookRot, Time.deltaTime);
    //    this.transform.rotation = rot;
    //    agent.SetDestination(patrol_point.position);
    //}
    #endregion
}
