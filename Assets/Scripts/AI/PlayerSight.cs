using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    GameObject targetPlayer = null;

    public bool hasPlayer()
    {
        return this.targetPlayer != null;
    }

    public GameObject getPlayer()
    {
        return this.targetPlayer;
    } 

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            this.targetPlayer = other.gameObject;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.targetPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        this.targetPlayer = null;
    }
}
