using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Target : MonoBehaviour
{
    PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
        {
            if (collision.gameObject.GetComponent<PlayerAttack>() ||
            collision.gameObject.GetComponent<Bullet>())
            {
                photonView.RPC("DestroyGameObject", RpcTarget.All);
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (photonView.IsMine)
        {
            if (collision.gameObject.GetComponent<PlayerAttack>() ||
                collision.gameObject.GetComponent<Bullet>())
            {
                photonView.RPC("DestroyGameObject", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    void DestroyGameObject()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
