﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeScript : MonoBehaviour
{
    [Tooltip("If this is false the player will do his balance on ledge animation, if true he'll grab the ledge")]
    [SerializeField]
    bool GrabLedge;
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            if(GrabLedge)
            {
                col.gameObject.GetComponent<PlayerMovement>().LedgeGrabbing = true;
                col.gameObject.GetComponent<PlayerMovement>().Ledgegrabbing(this.gameObject);
            }
            else
            {
                col.gameObject.GetComponent<PlayerMovement>().PlayAnimation("Balancing");
            }
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (GrabLedge)
        {
            col.gameObject.GetComponent<PlayerMovement>().LedgeGrabbing = false;
        }
        else
        {
            col.gameObject.GetComponent<PlayerMovement>().StopAnimation("Balancing");
        }
    }
}
