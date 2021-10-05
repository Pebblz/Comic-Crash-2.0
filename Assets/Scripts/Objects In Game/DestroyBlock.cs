using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class DestroyBlock : MonoBehaviour
{
    [SerializeField] float TimeToDestroy = 5f;

    private void Update()
    {
        if (PhotonFindCurrentClient().GetComponent<PhotonView>().IsMine)
        {
            if (TimeToDestroy < 0)
                PhotonNetwork.Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (FindObjectOfType<Builder>() != null)
        {
            Builder builder = FindObjectOfType<Builder>();
            builder.RemoveFromList(this);
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
