using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class DestroyBlock : MonoBehaviour
{
    [SerializeField] float TimeToDestroy = 5f;

    PhotonView photonView;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    private void Update()
    {
        TimeToDestroy -= Time.deltaTime;

        if (photonView.IsMine)
        {
            if (TimeToDestroy < 0)
                photonView.RPC("DestroyThis", RpcTarget.All);
        }
    }
    [PunRPC]
    void DestroyThis()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            if (FindObjectOfType<Builder>() != null)
            {
                Builder builder = FindObjectOfType<Builder>();
                builder.RemoveFromList(this);
            }
        }
    }
}
