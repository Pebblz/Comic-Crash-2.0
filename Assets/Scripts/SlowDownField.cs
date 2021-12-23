using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownField : MonoBehaviour
{
    [SerializeField]
    int SlowDownBy = 1;
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
            col.gameObject.GetComponent<PlayerMovement>().SlowDown(SlowDownBy);
    }
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
            col.gameObject.GetComponent<PlayerMovement>().SlowDown(SlowDownBy);
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
            col.gameObject.GetComponent<PlayerMovement>().ResetSpeed();
    }
}
