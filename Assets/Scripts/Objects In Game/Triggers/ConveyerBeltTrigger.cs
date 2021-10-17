using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBeltTrigger : MonoBehaviour
{
    ConveyerBelt ParentBelt;
    void Start()
    {
        ParentBelt = transform.parent.GetComponent<ConveyerBelt>();
    }

    #region Collsion methods

    private void OnTriggerEnter(Collider col)
    {
        //adds the objects on the conveyer belt to the belt list
        if (col.gameObject.tag != "Floor" && col.gameObject.tag != "Platform" && !ParentBelt.onBelt.Contains(col.gameObject))
        {
            ParentBelt.onBelt.Add(col.gameObject);
            if (col.gameObject.tag == "Player")
                col.gameObject.GetComponent<PlayerMovement>().onBelt = true;
        }
    }
    private void OnTriggerStay(Collider col)
    {
        //adds the objects on the conveyer belt to the belt list
        if (col.gameObject.tag != "Floor" && col.gameObject.tag != "Platform" && !ParentBelt.onBelt.Contains(col.gameObject))
        {
            ParentBelt.onBelt.Add(col.gameObject);
            if (col.gameObject.tag == "Player")
                col.gameObject.GetComponent<PlayerMovement>().onBelt = true;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        //removes the objects on the conveyer belt list
        if (col.gameObject.tag != "Floor" && col.gameObject.tag != "Platform")
        {
            ParentBelt.onBelt.Remove(col.gameObject);
            if (col.gameObject.tag == "Player")
                col.gameObject.GetComponent<PlayerMovement>().onBelt = false;
        }
    }
    #endregion
}
