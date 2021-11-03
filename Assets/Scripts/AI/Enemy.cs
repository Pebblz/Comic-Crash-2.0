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
        DEAD

    }

    protected Transform target;
    public STATE current_state = STATE.IDLE;
    public float attack_range = 2f;
    [Tooltip("The amount of health an enemy has")]
    public int health;
    PhotonView photonView;
    public int enemy_damage = 1;

    protected void Awake()
    {
        this.photonView = GetComponent<PhotonView>();
    }

    public void die()
    {
        photonView.RPC("DestroyGameObject", RpcTarget.All);

    }

    public void Update()
    {
        if(this.health <= 0)
        {
            this.current_state = STATE.DEAD;
        }
    }

    public void damage(int amount)
    {
        photonView.RPC("take_damage", RpcTarget.All, amount );
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

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.current_state = STATE.CHASE;
        }
    }

    protected void OnTriggerStay(Collider other)
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

    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.current_state = STATE.IDLE;
        }
    }
}
