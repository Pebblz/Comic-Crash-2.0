using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
public abstract class Enemy : MonoBehaviour
{
    [Tooltip("The amount of health an enemy has")]
    public float health;
    PhotonView photonView;

    protected void Awake()
    {
        this.photonView = GetComponent<PhotonView>();
    }

    public void die()
    {
        photonView.RPC("DestroyGameObject", RpcTarget.All);

    }

    public void damage(float amount)
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
    public void take_damage(float amount)
    {
        if (amount == null)
        {
            return;
        }

        this.health -= amount;
    }
    #endregion


}
