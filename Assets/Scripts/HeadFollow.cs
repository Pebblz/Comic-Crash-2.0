using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFollow : MonoBehaviour
{
    GameObject player;
    public GameObject head;
    Quaternion headStartingRot;
    bool lookAt;
    private void Start()
    {
        headStartingRot = head.transform.rotation;
    }
    void Update()
    {
        if (player == null)
        {
            //this needs to be here because the player can switch characters
            player = GameObject.FindGameObjectWithTag("Player");
        }

        //this checks if any of the rays hit an object with pickupables script
        if (lookAt)
        {
            head.transform.LookAt(player.transform.position);
        }
        else
        {
            head.transform.rotation = headStartingRot;
        }


    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag ==  "Player")
        {
            lookAt = true;
        }
    }
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            lookAt = true;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            lookAt = false;
        }

    }
}
