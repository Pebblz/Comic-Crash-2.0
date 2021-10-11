using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Bullet : MonoBehaviourPunCallbacks
{
    float TimeTillDestroy = 2;
    PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    void Update()
    {
        TimeTillDestroy -= Time.deltaTime;

        if (TimeTillDestroy <= 0)
        {
            if (GetComponent<PhotonView>().AmOwner)
            {
                photonView.RPC("DestroyGameObject", RpcTarget.All);
            }
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
    [PunRPC]
    void DestroyGameObject()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
