using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Bullet : MonoBehaviourPunCallbacks
{
    float TimeTillDestroy = 2;
    PhotonView photonView;
    Rigidbody body;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        body = GetComponent<Rigidbody>();
    }
    void Update()
    {
        TimeTillDestroy -= Time.deltaTime;
        //look the direction you're movig 
        transform.rotation = Quaternion.LookRotation(body.velocity);
        if (photonView.IsMine)
        {
            if (TimeTillDestroy <= 0)
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
