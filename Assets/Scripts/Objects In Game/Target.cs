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
    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            if (other.gameObject.tag == "PlayerPunch" ||
                other.gameObject.GetComponent<Bullet>())
            {
                photonView.RPC("DestroyGameObject", RpcTarget.All);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (photonView.IsMine)
        {
            if (other.gameObject.tag == "PlayerPunch" ||
                other.gameObject.GetComponent<Bullet>())
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
