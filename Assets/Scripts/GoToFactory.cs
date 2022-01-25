using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GoToFactory : MonoBehaviour
{
    [SerializeField] int RequiredAmountBolts = 1;
    [SerializeField] int FactoryLevelIndex;
    GameManager Gm;
    GameObject Player;
    void Start()
    {
        Gm = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            Player = PhotonFindCurrentClient();
            if(Player.GetComponent<PhotonView>().IsMine)
            {
                if(Gm.CollectibleCount >= RequiredAmountBolts)
                {
                    PhotonNetwork.LoadLevel(FactoryLevelIndex);
                    this.gameObject.SetActive(false);
                }
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
}
