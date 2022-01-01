using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class PatTrigger : MonoBehaviour
{
    [SerializeField] int LevelIndex;


    private void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "Player")
            PhotonNetwork.LoadLevel(LevelIndex);
    }
}
