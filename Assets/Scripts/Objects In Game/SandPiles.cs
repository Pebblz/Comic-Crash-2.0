using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
public class SandPiles : MonoBehaviour
{
    [SerializeField]
    GameObject Coin;
    [SerializeField, Range(.01f, 6f)]
    float HowFastToSquish = 3f;
    float SquishOffSet = 1f;
    [SerializeField]
    float TimerToDelete = .2f;
    PhotonView photonView;
    bool squish ;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (squish)
            {
                Squish();
                if (TimerToDelete <= 0)
                    photonView.RPC("DestroyBox", RpcTarget.All);

                TimerToDelete -= Time.deltaTime;
            }
        }
    }
    public void DestroyPile()
    {

        squish = true;
    }
    [PunRPC]
    void DestroyBox()
    {
        if (Coin != null)
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", Coin.name), transform.position + new Vector3(0, .5f, 0), transform.rotation);

        PhotonNetwork.Destroy(photonView);
    }
    void Squish()
    {
        transform.localScale -= new Vector3(0, HowFastToSquish, 0) * Time.deltaTime;
        transform.position -= new Vector3(0, SquishOffSet, 0) * Time.deltaTime;
    }
}
