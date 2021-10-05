using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviour
{
    float TimeTillDestroy = 2;
    void Update()
    {
        TimeTillDestroy -= Time.deltaTime;
        if (PhotonFindCurrentClient().GetComponent<PhotonView>().IsMine)
        {
            if (TimeTillDestroy <= 0)
            {
                PhotonNetwork.Destroy(this.gameObject);
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
