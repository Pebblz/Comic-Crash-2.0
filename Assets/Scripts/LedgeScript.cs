using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeScript : MonoBehaviour
{
    [SerializeField, Tooltip("If this is false the player will do his balance on ledge animation, if true he'll grab the ledge")]
    bool GrabLedge;
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            if(GrabLedge)
            {
                col.gameObject.GetComponent<PlayerMovement>().Ledge = this.gameObject;
                col.gameObject.GetComponent<PlayerMovement>().LedgeGrabbing = true;

            }
            else
            {
                col.gameObject.GetComponent<PlayerMovement>().PlayAnimation("Balancing");
            }
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (GrabLedge)
            {
                col.gameObject.GetComponent<PlayerMovement>().LedgeGrabbing = false;
                col.gameObject.GetComponent<PlayerMovement>().Ledge = null;
            }
            else
            {
                col.gameObject.GetComponent<PlayerMovement>().StopAnimation("Balancing");
            }
        }
    }
}
