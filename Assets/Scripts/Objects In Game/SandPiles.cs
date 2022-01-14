using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
public class SandPiles : MonoBehaviour
{
    [SerializeField]
    GameObject Coin;

    PhotonView photonView;

    public void DestroyPile()
    {
        photonView = GetComponent<PhotonView>();
        photonView.RPC("DestroyBox", RpcTarget.All);
    }
    [PunRPC]
    void DestroyBox()
    {
        if(Coin != null)
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", Coin.name), transform.position + new Vector3(0,.5f,0), transform.rotation);
        
        PhotonNetwork.Destroy(photonView);
    }
}
