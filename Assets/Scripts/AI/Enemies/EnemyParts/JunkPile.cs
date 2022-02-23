using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class JunkPile : MonoBehaviour
{
    public PhotonView photonView;
    public void BreakJunkPile()
    {
        photonView.RPC("DestroyGameObject", RpcTarget.All);
    }

    [PunRPC]
    void DestroyGameObject()
    {
        if(photonView.IsMine)
            PhotonNetwork.Destroy(photonView);
    }
}
