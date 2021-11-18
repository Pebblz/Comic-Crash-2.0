using Luminosity.IO;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
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
    bool doingSquish;

    PhotonView photonView;
    void Start()
    {
        origanalScale = transform.localScale;
        anim = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (InputManager.GetButton("Crouch") && !pm.OnGround && !pm.Swimming && pm.LongJumpTimer <= 0)
            {
                GroundPounding = true;
                pm.CantMove = true;
                GroundPound();
            }
            if (squishTime && pm.OnGround || squishTime && pm.OnGround)
            {
                StopAnimation("GroundPound");
                PlayAnimation("GroundPoundImpact");
                Squish();
            }

            //this is here to make sure he unSquishes
            //because if he jumps when he is unSquishing
            //he wont fully unSquish until he gets grounded again
            if (doingSquish && !pm.OnGround)
            {
                Squish();
            }
            if (pm.Swimming || pm.inWaterAndFloor)
            {
                StopAnimation("GroundPound");
                StopAnimation("GroundPoundImpact");
                pm.CantMove = false;
                doingSquish = false;
                GroundPounding = false;
                doneSquishing = false;
                squishTime = false;
                transform.localScale = origanalScale;
            }
        }
    }
    void GroundPound()
    {
        pm.GroundPound();
        squishTime = true;
        PlayAnimation("GroundPound");
        StopAnimation("GroundPoundImpact");
    }
    void Squish()
    {
        doingSquish = true;
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
                pm.CantMove = false;
                doingSquish = false;
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
