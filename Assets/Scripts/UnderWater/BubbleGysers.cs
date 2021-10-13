using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class BubbleGysers : MonoBehaviour
{
    [SerializeField]
    float bubbleTimer;

    float timer;

    [SerializeField]
    GameObject bubblePrefab;
    void Update()
    {
        if (timer <= 0)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", bubblePrefab.name), transform.position, transform.rotation);
            timer = bubbleTimer;
        }
        timer -= Time.deltaTime;
    }
}
