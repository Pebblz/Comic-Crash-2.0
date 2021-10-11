using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Bullet : MonoBehaviourPunCallbacks
{
    float TimeTillDestroy = 2;
    public PhotonView shootersView;
    PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    void Update()
    {
        TimeTillDestroy -= Time.deltaTime;
        if (shootersView.IsMine)
        {
            if (TimeTillDestroy <= 0)
            {
                photonView.RPC("DestroyGameObject",RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void DestroyGameObject()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
