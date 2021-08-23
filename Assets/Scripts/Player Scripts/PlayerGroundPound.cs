using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundPound : MonoBehaviour
{
    private Animator anim;
    PlayerMovement pm;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pm.OnGround && Input.GetButtonDown("Punch") 
            || Input.GetMouseButtonDown(0) && !pm.OnGround)
        {
            GroundPound();
        }
    }
    void GroundPound()
    {
        print("Pound Me");

        pm.GroundPound();
        
        //PlayAnimation("GroundPound");
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
