using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] int AmountOfAttacks;
    [SerializeField] GameObject[] PunchHitBoxes;
    private Animator anim;
    private int AttacksPreformed = 1;
    private float TimeTillnextAttack;
    private float TimeTillAttackReset;
    private bool AttackAgian;
    [HideInInspector] public bool CanAttack;
    PlayerMovement Pm;
    Rigidbody Rb;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Pm = GetComponent<PlayerMovement>();
        Rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //this is so the player can't swing around like a crazy person and kill everything around him
        if(TimeTillAttackReset > 0 && Pm.OnGround)
        {
            Pm.enabled = false;
            Rb.velocity = Vector3.zero;
        }
        if(TimeTillAttackReset <= 0)
        {
            StopAnimation("Attack");
            Pm.enabled = true;
            AttacksPreformed = 1;
        }
        TimeTillAttackReset -= Time.deltaTime;
        TimeTillnextAttack -= Time.deltaTime;
    }
    public void AttackPressed(InputAction.CallbackContext context)
    {
        //this is so if the players in any of
        //the jump animation you can't attack
        if (!anim.GetBool("Jump"))
        {
            if (AttacksPreformed == 1 && TimeTillnextAttack <= 0 && !Pm.isCrouching ||  
                AttacksPreformed == 1 && TimeTillnextAttack <= 0&& !Pm.isCrouching)
            {
                punch(AttacksPreformed);
            }
            //this basically queues up the next attack for the player
            if (AmountOfAttacks >= AttacksPreformed &&  TimeTillnextAttack <= 0|| 
                 AmountOfAttacks >= AttacksPreformed  && TimeTillnextAttack <= 0)
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
    void punch(int attackNumber)
    {
        Instantiate(PunchHitBoxes[attackNumber - 1], 
            transform.position + new Vector3(0,.6f,0) + transform.forward * 1.1f, Quaternion.identity);

        PlayAnimation("Attack",attackNumber);
        AttackAgian = false;
        //this is here to fix the end lag for his attack animations
        //---------------
        if(AttacksPreformed == 1)
        {
            TimeTillAttackReset = .6f;
        } else
        {
            TimeTillAttackReset = .85f;
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
    public void PlayAnimation(string BoolName,int AttackNumber)
    {
        anim.SetInteger(BoolName, AttackNumber);
    }
    /// <summary>
    /// Call this for anytime you need to stop an animation
    /// </summary>
    /// <param name="BoolName"></param>
    public void StopAnimation(string BoolName)
    {
        anim.SetInteger(BoolName, 0);
    }
    #endregion
}
