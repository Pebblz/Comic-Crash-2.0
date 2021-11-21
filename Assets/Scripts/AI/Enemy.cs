using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
public abstract class Enemy : MonoBehaviour
{
    public enum STATE
    {
        IDLE,
        CHASE,
        ATTACK,
        FLEE,
        DEAD, 
        STUN

    }

    protected Transform target;
    public STATE current_state = STATE.IDLE;
    public float attack_range = 2f;
    [Tooltip("The amount of health an enemy has")]
    public int health;
    PhotonView photonView;
    public int enemy_damage = 1;
    public float stun_timer = 2f;
    private float init_stun_timer;
    public float iframes = 1f;
    private float init_iframes;
    public bool hit = false;

    protected virtual void Awake()
    {
        this.photonView = GetComponent<PhotonView>();
        init_stun_timer = stun_timer;
        init_iframes = iframes;
        hit = false;
    }

    public void die()
    {
        this.current_state = STATE.DEAD;
        //photonView.RPC("DestroyGameObject", RpcTarget.All);

    }

    protected virtual void Update()
    {



        if(this.current_state == STATE.STUN)
        {
            this.current_state = STATE.STUN;
            stun_timer -= Time.deltaTime;
            if(stun_timer <= 0f)
            {
                this.current_state = STATE.IDLE;
                this.stun_timer = init_stun_timer;
            }

        }
        if (hit)
        {
            this.iframes -= Time.deltaTime;
            if(iframes <= 0)
            {
                this.hit = false;
                iframes = init_iframes;
            }

        }
        if(this.health <= 0)
        {
            this.current_state = STATE.DEAD;
        }

        if(this.current_state == STATE.DEAD)
        {

            this.gameObject.SetActive(false);
        }
    }

    

    public void damage(int amount)
    {
        if(hit == true)
        {
            return;
        }
        Debug.Log("Calling damage RPC");
        photonView.RPC("take_damage", RpcTarget.All, amount );
        hit = true;
    }


    /// <summary>
    /// Checks to see if point is in target's line of sight
    /// </summary>
    /// <param name="pos">Position to look at</param>
    /// <param name="layer">What layers are to be hit</param>
    /// <returns>True if intersected</returns>
    public bool is_in_los(Vector3 target) {
        return Physics.Linecast(this.transform.position, target);
    }

    #region RPCs
    [PunRPC]
    void DestroyGameObject()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void take_damage(int amount)
    {
        if (amount == null)
        {
            return;
        }

        this.health -= amount;
    }
    #endregion

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.current_state = STATE.CHASE;
        }
 
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Vector3.Distance(this.transform.position, other.transform.position) <= attack_range)
            {
                this.current_state = STATE.ATTACK;
            }
            else
            {
                this.current_state = STATE.CHASE;
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (this.current_state == STATE.STUN)
        {
            return;
        }
        if (other.gameObject.tag == "Player")
        {
            this.current_state = STATE.IDLE;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Shot")
        {
            this.current_state = STATE.STUN;
        }
    }
}
