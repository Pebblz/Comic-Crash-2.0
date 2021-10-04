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
        if (TimeTillDestroy <= 0)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
