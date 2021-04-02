using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] int AmountOfAttacks;
    [SerializeField] GameObject[] PunchHitBoxes;
    private Animator anim;
    private int AttacksPreformed = 1;
    private float TimeTillnextAttack;
    private float TimeTillAttackReset;
    private bool AttackAgian;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) &&  AttacksPreformed == 1 && TimeTillnextAttack <= 0
           && !Input.GetKey(KeyCode.C))
        {
            punch(AttacksPreformed);
        }
        //this basically queues up the next attack
        if(Input.GetMouseButtonDown(0) && AmountOfAttacks >= AttacksPreformed && !Input.GetKey(KeyCode.C))
        {
            AttackAgian = true;
        }
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Punch" + (AttacksPreformed - 1)) &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f && AttackAgian)
        {
            punch(AttacksPreformed);
        }
        //this is so the player can't swing around like a crazy person and kill everything around him
        if(TimeTillAttackReset > 0 && GetComponent<PlayerMovement>().IsGrounded())
        {
            GetComponent<PlayerMovement>().enabled = false;
        }
        if(TimeTillAttackReset <= 0)
        {
            GetComponent<PlayerMovement>().enabled = true;
            AttacksPreformed = 1;
            StopAnimation("Attack");
        }
        TimeTillAttackReset -= Time.deltaTime;
        TimeTillnextAttack -= Time.deltaTime;
    }
    void punch(int attackNumber)
    {
        //GameObject temp = Instantiate(PunchHitBoxes[attackNumber],transform);
        PlayAnimation("Attack",attackNumber);
        AttackAgian = false;
        AttacksPreformed++;
        TimeTillnextAttack = .5f;
        TimeTillAttackReset = .6f;
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
