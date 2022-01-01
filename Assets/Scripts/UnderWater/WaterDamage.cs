using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class WaterDamage : MonoBehaviour
{
    [SerializeField]
    int damage;

    [SerializeField]
    bool damageOnHit;

    float timer = 3;

    bool startTimer;

    GameObject Player;

    private void Update()
    {
        if (startTimer)
            timer -= Time.deltaTime;
        if (timer < 0)
        {
            if (Player != null)
                Player.GetComponent<PlayerHealth>().HurtPlayer(damage);

            startTimer = false;
            timer = 3;
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (PhotonFindCurrentClient().GetComponent<PhotonView>().IsMine)
        {
            if (damageOnHit)
            {
                if (col.gameObject.GetComponent<BlobBert>())
                {
                    col.GetComponent<PlayerHealth>().HurtPlayer(damage);
                }
            }
            else
            {
                if (col.gameObject.GetComponent<BlobBert>())
                {
                    Player = col.gameObject;
                    startTimer = true;
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
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (PhotonFindCurrentClient().GetComponent<PhotonView>().IsMine)
            {
                if (damageOnHit)
                {
                    if (col.gameObject.GetComponent<BlobBert>())
                    {
                        col.GetComponent<PlayerHealth>().HurtPlayer(damage);
                    }
                }
                else
                {
                    if (col.gameObject.GetComponent<BlobBert>())
                    {
                        Player = col.gameObject;
                        startTimer = true;
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (PhotonFindCurrentClient().GetComponent<PhotonView>().IsMine)
        {
            if (col.gameObject.GetComponent<BlobBert>())
            {
                startTimer = false;
                timer = 3;
                Player = null;
            }
        }
    }
}
