using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentPlayer : MonoBehaviour
{
    [SerializeField]
    bool onlyOnTop;
    private void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == "Player" && !onlyOnTop)
        {
            col.transform.parent = transform;
        }
        if(col.gameObject.tag == "Player" && onlyOnTop)
        {
           if( col.transform.position.y > transform.position.y)
            {
                col.transform.parent = transform;
            }
        }
    }
    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.transform.parent = null;
        }
    }
}
