using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideWhenCollidedWith : MonoBehaviour
{
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerMovement>().enabled = false;
        }
    }
    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerMovement>().enabled = true;
        }
    }
}
