using UnityEngine;
using UnityEngine.AI;
using Photon.Realtime;
using Photon.Pun;
using Luminosity.IO;
public class Pet : MonoBehaviour
{
    NavMeshAgent agent;

    States currentState;
    IdleAnimationToPlay IdleToPlay;
    Animator anim;
    float IdleTimer;
    [SerializeField]
    float minIdleTimer = 1f, maxIdleTimer = 5f, distanceAwayToPet = 2;

    public Transform player;

    public LayerMask Ground;

    void Start()
    {
        SetUpIdle();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = PhotonFindCurrentClient().transform;
        }
        else
        {
            if (Vector3.Distance(transform.position, player.position) <= distanceAwayToPet && InputManager.GetButtonDown("Interact") &&
                !anim.GetCurrentAnimatorStateInfo(0).IsName("Petting"))
            {
                PlayAnimation("Petting");
                currentState = States.GettingPet;
            }
        }
            if (currentState == States.Idle)
            {
                if(IdleTimer <= 0)
                {
                    //add pathfinding code here 

                    currentState = States.Walking;
                }

                IdleTimer -= Time.deltaTime;
            }
            if (currentState == States.Walking)
            {
            PlayAnimation("Walking");


            StopAnimation("Walking");
                SetUpIdle();
            }
            if (currentState == States.Running)
            {
                SetUpIdle();
            }
            if (currentState == States.GettingPet)
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Petting"))
                {
                    StopAnimation("Petting");
                    SetUpIdle();
                }
            }
        
    }
    void SetUpIdle()
    {
        GetRandomIdleAnimation();
        IdleTimer = RandomNumber(minIdleTimer, maxIdleTimer);
        currentState = States.Idle;
    }
    float RandomNumber(float min, float max)
    {
        return Random.Range(min, max);
    }
    float ReturnDistance(Vector3 Point1, Vector3 Point2)
    {
        return Vector3.Distance(Point1, Point2);
    }
    void GetRandomIdleAnimation()
    {
        IdleToPlay = (IdleAnimationToPlay)Random.Range(0, 2);

        if(IdleToPlay == IdleAnimationToPlay.sitting)
        {
            PlayAnimation("Sitting");
        }
        if(IdleToPlay == IdleAnimationToPlay.LayingDown)
        {
            PlayAnimation("LayingDown");
        }
        if(IdleToPlay == IdleAnimationToPlay.Sleeping)
        {
            PlayAnimation("Sleeping");
        }
    }
    GameObject PhotonFindCurrentClient()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject g in players)
        {
            if (g.GetComponent<PhotonView>().IsMine)
                return g;
        }
        return null;
    }
    enum States
    {
        Idle,
        Walking,
        Running,
        AtHomePoint,
        FallowingPlayer,
        GettingPet,
        RubbingBelly,
        JumpingDown,
        JumpUp
    }
    enum IdleAnimationToPlay
    {
        sitting,
        LayingDown,
        Sleeping
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
    public void SetAnimatorFloat(string FloatName, float speed)
    {
        anim.SetFloat(FloatName, speed);
    }
    #endregion
}
