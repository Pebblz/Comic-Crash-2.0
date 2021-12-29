using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class CollideWithHeavyBlock : MonoBehaviour
{
    Transform player;

    private void Update()
    {
        if (player == null)
            player = PhotonFindCurrentClient().transform;
        else
            Physics.IgnoreCollision(GetComponent<Collider>(), player.gameObject.GetComponent<Collider>());
    }

    
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "HeavyObject")
            Physics.IgnoreCollision(col.collider, gameObject.GetComponent<Collider>());
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
