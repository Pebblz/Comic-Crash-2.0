using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField]
    float BounceDistance;

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerMovement>().jumpOnBouncePad(BounceDistance);
        }
    }
    //this is here for a failsafe just incase the player doesn't want to bounce with on trigger enter 
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerMovement>().jumpOnBouncePad(BounceDistance);
        }
    }
}
