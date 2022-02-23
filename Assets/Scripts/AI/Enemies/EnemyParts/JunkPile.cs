using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class JunkPile : MonoBehaviour
{
    PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void BreakJunkPile()
    {
        print("Destroying Pile");
        photonView.RPC("DestroyGameObject", RpcTarget.All);
    }

    [PunRPC]
    void DestroyGameObject()
    {
        if(photonView.IsMine)
            PhotonNetwork.Destroy(photonView);
    }
}
