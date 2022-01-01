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

    GameObject player;
    SoundManager sound;
    PhotonView photonView;

    private void Start()
    {
            sound = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }
    void Update()
    {
        if (player == null)
            player = PhotonFindCurrentClient();
        // Rotate the object around its local y axis at 1 degree per second
        transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        photonView = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            GameObject gm = FindObjectOfType<GameManager>().gameObject;

            if (collect == collectible.Coin)
            {
                sound.playCoin(this.transform.position);
                gm.GetComponent<GameManager>().coinCount += numberGivenToPlayer;
                photonView.RPC("DestroyThis", RpcTarget.All);
            }
            else if (collect == collectible.MainCollectible)
            {
                gm.GetComponent<GameManager>().CollectibleCount += numberGivenToPlayer;
                if (col.gameObject == player)
                {
                    FindObjectOfType<PlayerMovement>().CollectibleGotten = true;
                    photonView.RPC("DestroyThis", RpcTarget.All);
                }
            }
            else if (collect == collectible.HeartOne)
            {
                if (col.gameObject == player)
                {
                    if (col.gameObject.GetComponent<PlayerHealth>().currentHealth != col.gameObject.GetComponent<PlayerHealth>().maxHealth)
                    {
                        col.gameObject.GetComponent<PlayerHealth>().currentHealth += 1;
                        photonView.RPC("DestroyThis", RpcTarget.All);
                    }
                }
            }
            else if (collect == collectible.maxHealthUp)
            {
                if (col.gameObject == player)
                {
                    col.gameObject.GetComponent<PlayerHealth>().maxHealth += 1;
                    col.gameObject.GetComponent<PlayerHealth>().ResetHealth();
                    print("new health = " + col.gameObject.GetComponent<PlayerHealth>().maxHealth);
                    photonView.RPC("DestroyThis", RpcTarget.All);
                }
            }
            else if (collect == collectible.FullHeal)
            {
                if (col.gameObject == player)
                {
                    if (col.gameObject.GetComponent<PlayerHealth>().currentHealth != col.gameObject.GetComponent<PlayerHealth>().maxHealth)
                    {
                        col.gameObject.GetComponent<PlayerHealth>().ResetHealth();
                        photonView.RPC("DestroyThis", RpcTarget.All);
                    }
                }
            }

        }
    }
    [PunRPC]
    void DestroyThis()
    {
        if(photonView.IsMine)
            PhotonNetwork.Destroy(this.gameObject);
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
    enum collectible
    {
        Coin,
        MainCollectible,
        HeartOne,
        FullHeal,
        maxHealthUp
    }
}
