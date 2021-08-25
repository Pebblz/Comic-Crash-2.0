using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundPound : MonoBehaviour
{
    private Animator anim;
    PlayerMovement pm;
    [SerializeField, Range(.1f, 6f)]
    float timeToSquish;
    private Vector3 origanalScale;
    private bool squishTime;
    private bool doneSquishing;
    public bool GroundPounding;
    void Start()
    {
        origanalScale = transform.localScale;
        anim = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pm.OnGround && Input.GetButtonDown("Punch") 
            || Input.GetMouseButtonDown(0) && !pm.OnGround)
        {
            GroundPounding = true;
            GroundPound();
        }
        if (squishTime && pm.OnGround)
        {
            StopAnimation("GroundPound");
            PlayAnimation("GroundPoundImpact");
            Squish();
        }
    }
    void GroundPound()
    {
        pm.GroundPound();
        squishTime = true;
        PlayAnimation("GroundPound");
    }
    void Squish()
    {
        
        if (!doneSquishing)
        {
            if (transform.localScale.y > origanalScale.y / 2f)
            {
                transform.localScale -= new Vector3(-3, timeToSquish, -3) * Time.deltaTime;
            }
            else
            {
                doneSquishing = true;
            }
        }
        else
        {
            StopAnimation("GroundPoundImpact");
            if (transform.localScale.y < origanalScale.y)
            {
                transform.localScale += new Vector3(-3, timeToSquish, -3) * Time.deltaTime;
            }
            else
            {
                GroundPounding = false;
                doneSquishing = false;
                squishTime = false;
            }
        }
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
