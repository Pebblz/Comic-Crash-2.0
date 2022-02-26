using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class SpiderGoop : Enemy, IRespawnable
{
    #region Vars
    [SerializeField]
    private float walkRotSpeed = 2, chaseRotSpeed = 3, idleMoveSpeed, chaseSpeed, chargeSpeed,
        timeTillIdle, playerSeenRange, chargeCooldown, chargeDuration,
        firstSeenPlayer, distAwayToCharge = 8, stunDuration, maxIFrameTime;

    int damage = 1, health = 3;

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

    float chargeCooldownTimer, chargeDurationTimer, firstSeenPlayerTimer, stunTimer,
        IFrameTimer;

    bool jumping, charging, stunned, dead;

    PhotonView photonView;

    #endregion

    private void Start()
    {
        anim = GetComponent<Animator>();
        Detection = GetComponent<EnemyDetection>();
        startingPoint = transform.position;
        startingRot = transform.rotation;
        rb = GetComponent<Rigidbody>();
        chargeDurationTimer = chargeDuration;
        firstSeenPlayerTimer = firstSeenPlayer;
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (!charging && !stunned && !dead)
            {
                if (Detection.IsPlayerInSight()
                    || detectedPlayers.Count > 0
                    || lastSeenPlayerPos != Vector3.zero)
                {
                    anim.SetBool("Idle", false);
                    anim.SetBool("Walking", true);
                    ChasePlayer();
                    firstSeenPlayerTimer -= Time.deltaTime;
                }
                else
                {
                    if (idleWays == WaysToIdle.StandAtPoint)
                    {
                        if (Vector3.Distance(transform.position, startingPoint) > 1.5f)
                        {
                            if (timerTillIdle > 0)
                            {
                                anim.SetBool("Idle", true);
                                anim.SetBool("Walking", false);
                                timerTillIdle -= Time.deltaTime;
                            }
                            else
                            {
                                anim.SetBool("Idle", false);
                                anim.SetBool("Walking", true);
                                ReturnToStartingPoint();
                            }
                        }
                        else
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, startingRot, 3 * Time.deltaTime);
                            anim.SetBool("Idle", true);
                            anim.SetBool("Walking", false);
                        }
                    }
                    if (idleWays == WaysToIdle.WayPoint)
                    {
                        if (timerTillIdle > 0)
                        {
                            anim.SetBool("Idle", true);
                            anim.SetBool("Walking", false);
                            timerTillIdle -= Time.deltaTime;
                        }
                        else
                        {
                            anim.SetBool("Idle", false);
                            anim.SetBool("Walking", true);
                            WaypointMovement();
                        }
                    }
                }
                chargeCooldownTimer -= Time.deltaTime;
            }
            else
            {
                if (charging)
                {
                    anim.SetBool("Charge", true);
                    if (anim.GetAnimatorTransitionInfo(0).IsName("Start_Charge -> Charging") || anim.GetCurrentAnimatorStateInfo(0).IsName("Charging"))
                    {
                        ChargeAtPlayer();
                        chargeDurationTimer -= Time.deltaTime;
                        if (chargeDurationTimer <= 0)
                        {
                            ResetCharge();
                        }
                    }
                }
                if (stunned)
                {
                    anim.SetBool("Charge", false);
                    anim.SetBool("Stunned", true);
                    if (anim.GetAnimatorTransitionInfo(0).IsName("Stun 1 -> Stun 2") || anim.GetCurrentAnimatorStateInfo(0).IsName("Stun 2"))
                    {
                        stunTimer -= Time.deltaTime;
                        if (stunTimer <= 0)
                        {
                            anim.SetBool("Stunned", false);
                            anim.SetBool("UnStunned", true);
                        }
                    }
                    if (anim.GetAnimatorTransitionInfo(0).IsName("Stun 3 -> Idle"))
                    {
                        ResetCharge();
                        anim.SetBool("UnStunned", false);
                        anim.SetBool("Stunned", false);
                        anim.SetBool("Idle", true);
                        stunned = false;
                    }
                }
            }
            //plays the dead anim
            if (health <= 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
            {
                anim.SetBool("Dead", true);
                dead = true;
            }
            //turns off the enemy when dead's over
            if (anim.GetAnimatorTransitionInfo(0).IsName("Dead -> Walk"))
            {
                anim.SetBool("Dead", false);
                //do this after death anim
                gameObject.SetActive(false);
            }
            IFrameTimer -= Time.deltaTime;
        }
    }

    private void ChasePlayer()
    {
        if (detectedPlayers.Count == 0)
        {
            detectedPlayers = Detection.GetPlayersInSight();
            if (lastSeenPlayerPos != Vector3.zero && !charging)
            {
                //this is here so the enemy wont have a point to go to that's in the air
                Vector3 ChasedPlayerpos = new Vector3(lastSeenPlayerPos.x, transform.position.y, lastSeenPlayerPos.z);

                //rotation
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                  Quaternion.LookRotation(ChasedPlayerpos - transform.position),
                                  chaseRotSpeed * Time.deltaTime);

                if (!ShouldJump(lastSeenPlayerPos))
                {
                    //Dot(.9f, ChasedPlayerpos, false) &&
                    //if dot's 1 then the enemies looking at the vector3 pos if -1 enemies looking the wrong way
                    if ( !jumping)
                    {
                        rb.velocity = (transform.forward * chaseSpeed) + new Vector3(0, rb.velocity.y, 0);
                    }
                }
                else if (!jumping)
                {

                    anim.SetBool("Jump", true);
                    anim.SetBool("OnGround", false);
                }
                //if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !anim.GetAnimatorTransitionInfo(0).IsName("Jump -> Walk") && !jumping)
                //{
                //    jumping = true;
                //    rb.velocity = (transform.forward) + new Vector3(0, GetJumpHeight(lastSeenPlayerPos.y));
                //}
                //if the enemies at the last seen pos
                if (Vector3.Distance(transform.position, ChasedPlayerpos) < .3f || lastSeenPlayerPos.y > transform.position.y)
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

                    firstSeenPlayerTimer = firstSeenPlayer;
                    //then do this
                    detectedPlayers.Clear();
                    chasedPlayer = null;
                }
                else
                {
                    //Rotates slowly towards player
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(ChasedPlayersPos - transform.position),
                                                      chaseRotSpeed * Time.deltaTime);

                    if (!ShouldJump(chasedPlayer.transform.position))
                    {
                        //Moves enemy towards player
                        if (Vector3.Distance(transform.position, chasedPlayer.transform.position) > 2.5f && !jumping && !charging)
                            rb.velocity = (transform.forward * chaseSpeed) + new Vector3(0, rb.velocity.y, 0);
                        //charges at player
                        if (Vector3.Distance(transform.position, chasedPlayer.transform.position) <= distAwayToCharge && !jumping && chargeCooldownTimer <= 0 &&
                            firstSeenPlayerTimer <= 0)
                            charging = true;
                        //if he's on top of the player i just want him to ignore the cooldown timer and attack
                        if (Vector3.Distance(transform.position, chasedPlayer.transform.position) <= 2.5f && !jumping && Dot(.95f, ChasedPlayersPos, false))
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
                        rb.velocity = (transform.forward) + new Vector3(0, GetJumpHeight(chasedPlayer.transform.position.y));
                    }
                }
            }
        }
        if (rb.velocity.y > -.2f && rb.velocity.y < .2f && jumping)
        {
            print("OnGround");
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
                                              walkRotSpeed * Time.deltaTime);
        //Moves enemy towards startPos
        rb.velocity = transform.forward * idleMoveSpeed;
    }

    void WaypointMovement()
    {
        if (waypoints.Count > 0)
        {
            //this is here just so we don't have the enemy fly
            Vector3 currentWaypointPos = new Vector3(waypoints[currentWaypointIndex].transform.position.x, transform.position.y, waypoints[currentWaypointIndex].transform.position.z);

            if (Vector3.Distance(transform.position, currentWaypointPos) > .4f)
            {

                //Rotates slowly towards waypoint
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(currentWaypointPos - transform.position),
                                                      walkRotSpeed * Time.deltaTime);

                //if (!ShouldJump(waypoints[currentWaypointIndex].transform.position))
                //{
                //Moves enemy towards waypoint
                rb.velocity = (transform.forward + new Vector3(0, rb.velocity.y, 0)) * idleMoveSpeed;
                //}
                //else if (!jumping)
                //{
                //    anim.SetBool("Jump", true);
                //    anim.SetBool("OnGround", false);
                //}
                //if (anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !anim.GetAnimatorTransitionInfo(0).IsName("Jump -> Walk") && !jumping)
                //{
                //    jumping = true;
                //    rb.velocity = (transform.forward) + new Vector3(0, GetJumpHeight(waypoints[currentWaypointIndex].transform.position.y));
                //}
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
        anim.SetBool("Idle", false);
        rb.velocity = (transform.forward + new Vector3(0, rb.velocity.y, 0)) * chargeSpeed;
    }

    void DamagePlayer(GameObject AttackedPlayer)
    {
        PlayerHealth Player = AttackedPlayer.GetComponent<PlayerHealth>();
        if (Player != null)
        {
            Vector3 dir = AttackedPlayer.transform.position - transform.position;
            //AttackedPlayer.GetComponent<PlayerMovement>().Knockback(dir,-200,2);
            //makes sure spider doesn't chase player when he respawns
            Player.HurtPlayer(damage);
            if (AttackedPlayer.GetComponent<PlayerDeath>().isdead)
            {
                if (detectedPlayers.Contains(AttackedPlayer))
                {
                    detectedPlayers.Remove(AttackedPlayer);
                }
                chasedPlayer = null;
            }
        }

    }

    void ResetCharge()
    {
        anim.SetBool("Charge", false);
        chargeCooldownTimer = chargeCooldown;
        chargeDurationTimer = chargeDuration;
        charging = false;
    }

    bool Dot(float distAway, Vector3 OtherPos, bool ChangePosY)
    {
        if (ChangePosY)
            OtherPos = new Vector3(OtherPos.x, transform.position.y, OtherPos.z);
        //the direction of the last seen pos 
        Vector3 dir = (OtherPos - transform.position).normalized;
        //this sees if the enemies looking at the last seen pos
        float dot = Vector3.Dot(dir, transform.forward);

        if (dot > distAway)
            return true;
        else
            return false;
    }

    bool ShouldJump(Vector3 HuntedTarget)
    {
        if (HuntedTarget.y > transform.position.y)
        {
            Vector3 start = this.gameObject.transform.position + new Vector3(0, .5f, 0);
            RaycastHit hit;
            bool TopHit = false;
            if (Dot(.9f, HuntedTarget, true))
            {
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
                                    if (hit.collider.gameObject.tag != "Player")
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

    private void OnTriggerStay(Collider col)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Charging") && col.gameObject.tag == "Player")
        {
            //also knockback player
            DamagePlayer(col.gameObject);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Charging") && col.gameObject.tag != "Player" 
            && !col.isTrigger && col.gameObject.tag != "Shot")
        {
                if (gameObject.transform.parent == null || gameObject.transform.parent != this.gameObject)
                {
                    //make here for stunning the enemy when hitting a wall
                    stunTimer = stunDuration;
                    detectedPlayers = Detection.GetPlayersInSight();
                    stunned = true;
                }
        }
        if (photonView.IsMine)
        {
            if (col.gameObject.GetComponent<Bullet>() && IFrameTimer <= 0 ||
            col.gameObject.tag == "PlayerPunch" && IFrameTimer <= 0)
            {
                health -= 1;
                IFrameTimer = maxIFrameTime;
            }
        }
    }

    public void reset_data()
    {
        health = 3;
        detectedPlayers.Clear();
        chasedPlayer = null;
        transform.position = startingPoint;
        transform.rotation = startingRot;
        lastSeenPlayerPos = Vector3.zero;
        chargeDurationTimer = chargeDuration;
        currentWaypointIndex = 0;
        jumping = false;
        stunned = false;
        charging = false;
        dead = false;
    }

    enum WaysToIdle
    {
        StandAtPoint,
        WayPoint
    }

}
