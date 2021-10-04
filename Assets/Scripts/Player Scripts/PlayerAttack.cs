using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Photon.Realtime;
using Photon.Pun;
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] int AmountOfAttacks;
    [SerializeField] GameObject[] PunchHitBoxes;
    [SerializeField] float[] AttackTimers = new float[3];
    [SerializeField] float AirAttackTimer;
    [SerializeField] float[] MoveForward = new float[3];
    private Animator anim;
    private int AttacksPreformed = 1;
    private float TimeTillnextAttack;
    private float TimeTillAttackReset;
    private bool AttackAgian;
    [HideInInspector] public bool CanAttack;
    [HideInInspector] public bool AirAttacked;
    HandMan handman;
    Rigidbody body;
    PlayerMovement movement;


    PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
        if (GetComponent<HandMan>())
            handman = GetComponent<HandMan>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        #region Punch
        //this is so if the players in any of
        //the jump animation you can't attack
        if (photonView.IsMine)
        {
            if (!anim.GetBool("Jump"))
            { // Ground Punch
                if (handman == null)
                {
                    if (InputManager.GetButtonDown("Left Mouse") && AttacksPreformed == 1 && TimeTillnextAttack <= 0
                       && !Input.GetKey(KeyCode.C))
                    {
                        punch(AttacksPreformed);
                    }
                    //this basically queues up the next attack for the player
                    if (InputManager.GetButtonDown("Left Mouse") && AmountOfAttacks >= AttacksPreformed && !Input.GetKey(KeyCode.C) && TimeTillnextAttack <= 0)
                    {
                        AttackAgian = true;
                    }
                    //this waits for the animation to be done before going to the next punch
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Punch" + (AttacksPreformed - 1)) &&
                        anim.GetCurrentAnimatorStateInfo(0).normalizedTime < .90f && AttackAgian)
                    {
                        punch(AttacksPreformed);
                    }
                }
                else
                {
                    if (!handman.isHoldingOBJ)
                    {
                        if (InputManager.GetButtonDown("Left Mouse") && AttacksPreformed == 1 &&
                            TimeTillnextAttack <= 0 && !Input.GetKey(KeyCode.C))
                        {
                            punch(AttacksPreformed);
                        }
                        //this basically queues up the next attack for the player
                        if (InputManager.GetButtonDown("Left Mouse") && AmountOfAttacks >= AttacksPreformed && !Input.GetKey(KeyCode.C) && TimeTillnextAttack <= 0)
                        {
                            AttackAgian = true;
                        }
                        //this waits for the animation to be done before going to the next punch
                        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Punch" + (AttacksPreformed - 1)) &&
                            anim.GetCurrentAnimatorStateInfo(0).normalizedTime < .90f && AttackAgian)
                        {
                            punch(AttacksPreformed);
                        }
                    }
                }
            }
            else
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Dive") || !anim.GetCurrentAnimatorStateInfo(0).IsName("GPFalling"))
                {
                    // Air Punch
                    if (handman == null)
                    {
                        if (InputManager.GetButtonDown("Left Mouse") && AttacksPreformed == 1 && TimeTillnextAttack <= 0)
                        {
                            PunchAir();
                        }
                    }
                    else
                    {
                        if (!handman.isHoldingOBJ)
                        {
                            if (InputManager.GetButtonDown("Left Mouse") && AttacksPreformed == 1 && TimeTillnextAttack <= 0)
                            {
                                PunchAir();
                            }
                        }
                    }
                }
            }
            #endregion
            //this is so the player can't swing around like a crazy person and kill everything around him
            if (TimeTillAttackReset > 0 && movement.OnGround)
            {
                movement.enabled = false;
                body.velocity = Vector3.zero;
            }

            //if (TimeTillAttackReset > 0 && !movement.OnGround)
            //{
            //    body.velocity = new Vector3(body.velocity.x / 2, -1, body.velocity.z / 2);
            //}

            if (TimeTillAttackReset <= 0 && movement.OnGround)
            {
                StopAnimationInt("Attack");
                StopAnimationBool("AirAttack");
                movement.enabled = true;
                AirAttacked = false;
                AttacksPreformed = 1;
            }
            if (movement.OnGround && AirAttacked)
            {
                StopAnimationBool("AirAttack");
                StopAnimationBool("Jump");
            }
            if (TimeTillAttackReset <= 0 && !movement.OnGround && AirAttacked)
            {
                StopAnimationBool("AirAttack");
                AirAttacked = false;
                movement.PlayFallingAnimation();
            }
        }
        TimeTillAttackReset -= Time.deltaTime;
        TimeTillnextAttack -= Time.deltaTime;
    }
    void PunchAir()
    {
        if(handman)
        {
            GameObject temp = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", PunchHitBoxes[0].name), transform.position + new Vector3(0,.6f,0) + transform.forward * 1.1f, Quaternion.identity);
            temp.transform.parent = transform;
        }
        else
        {
            GameObject temp = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", PunchHitBoxes[0].name), transform.position + transform.forward * 1.1f, Quaternion.identity);
            temp.transform.parent = transform;
        }


        PlayAnimation("AirAttack");
        TimeTillAttackReset = AirAttackTimer;
        AttacksPreformed = 2;
        AirAttacked = true;
        TimeTillnextAttack = .1f;
    }
    void punch(int attackNumber)
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", PunchHitBoxes[attackNumber - 1].name),
            transform.position + new Vector3(0, .6f, 0) + transform.forward * 1.1f, Quaternion.identity);

        PlayAnimation("Attack", attackNumber);

        AttackAgian = false;

        //this is here to fix the end lag for his attack animations
        //---------------
        if (AttacksPreformed == 1)
        {
            TimeTillAttackReset = AttackTimers[0];
            body.position += transform.forward * MoveForward[0] * Time.deltaTime;
        }
        if (AttacksPreformed == 2)
        {
            TimeTillAttackReset = AttackTimers[1];
            body.position += transform.forward * MoveForward[1] * Time.deltaTime;
        }
        if (AttacksPreformed == 3)
        {
            TimeTillAttackReset = AttackTimers[2];
            body.position += transform.forward * MoveForward[2] * Time.deltaTime;
        }
        //---------------
        AttacksPreformed++;
        TimeTillnextAttack = .1f;

    }
    #region Animation
    /// <summary>
    /// Call this for anytime you need to play an animation 
    /// </summary>
    /// <param name="animName"></param>
    public void PlayAnimation(string BoolName, int AttackNumber)
    {
        anim.SetInteger(BoolName, AttackNumber);
    }
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
    public void StopAnimationInt(string BoolName)
    {
        anim.SetInteger(BoolName, 0);
    }
    /// <summary>
    /// Call this for anytime you need to stop an animation
    /// </summary>
    /// <param name="BoolName"></param>
    public void StopAnimationBool(string BoolName)
    {
        anim.SetBool(BoolName, false);
    }
    #endregion
}
