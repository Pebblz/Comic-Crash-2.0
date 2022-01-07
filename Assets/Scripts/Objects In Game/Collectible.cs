using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class Collectible : MonoBehaviour
{
    [SerializeField, Tooltip("This is here for all the diffrent collectibles in the game")]
    collectible collect;

    [SerializeField, Range(1, 5), Tooltip("This would be here for if we have like a gold coin that would give 5 coins instead of 1")]
    int numberGivenToPlayer;

    [SerializeField, Range(0, 100)] float RotationSpeed;

    SoundManager sound;
    PhotonView photonView;

    private void Start()
    {
        sound = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }
    void Update()
    {

        // Rotate the object around its local y axis at 1 degree per second
        transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent<PlayerMovement>(out var player))
        {

            if (collect == collectible.Coin)
            {
                GameObject gm = FindObjectOfType<GameManager>().gameObject;
                photonView = GetComponent<PhotonView>();
                sound.playCoin(this.transform.position);
                gm.GetComponent<GameManager>().coinCount += numberGivenToPlayer;
                photonView.RPC("DestroyThis", RpcTarget.All);
            }
            else if (collect == collectible.MainCollectible)
            {
                GameObject gm = FindObjectOfType<GameManager>().gameObject;
                photonView = GetComponent<PhotonView>();
                gm.GetComponent<GameManager>().CollectibleCount += numberGivenToPlayer;
                player.CollectibleGotten = true;
                photonView.RPC("DestroyThis", RpcTarget.All);
            }
            else if (collect == collectible.HeartOne)
            {
                photonView = GetComponent<PhotonView>();
                col.gameObject.GetComponent<PlayerHealth>().currentHealth += 1;
                photonView.RPC("DestroyThis", RpcTarget.All);

            }
            else if (collect == collectible.maxHealthUp)
            {
                photonView = GetComponent<PhotonView>();
                col.gameObject.GetComponent<PlayerHealth>().maxHealth += 1;
                col.gameObject.GetComponent<PlayerHealth>().ResetHealth();
                photonView.RPC("DestroyThis", RpcTarget.All);

            }
            else if (collect == collectible.FullHeal)
            {
                photonView = GetComponent<PhotonView>();
                col.gameObject.GetComponent<PlayerHealth>().ResetHealth();
                photonView.RPC("DestroyThis", RpcTarget.All);

            }

        }
    }
    [PunRPC]
    void DestroyThis()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(this.gameObject);
    }

    enum collectible
    {
        Coin,
        MainCollectible,
        HeartOne,
        FullHeal,
        maxHealthUp
    }
}
