using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class PatTrigger : MonoBehaviour
{
    [SerializeField] int LevelIndex;

    private void OnTriggerEnter(Collider col)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (col.gameObject.tag == "Player")
            {
                PhotonNetwork.LoadLevel(LevelIndex);
                this.gameObject.SetActive(false);
            }
        }
    }
}
