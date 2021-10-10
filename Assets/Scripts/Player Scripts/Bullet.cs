using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Bullet : MonoBehaviourPunCallbacks
{
    float TimeTillDestroy = 2;
    public PhotonView shootersView;
    void Update()
    {
        TimeTillDestroy -= Time.deltaTime;
        if (shootersView.IsMine)
        {
            if (TimeTillDestroy <= 0)
            {
                shootersView.RPC("DestroyGameObject",RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void DestroyGameObject()
    {
        PhotonNetwork.Destroy(GetComponent<PhotonView>());
    }
}
