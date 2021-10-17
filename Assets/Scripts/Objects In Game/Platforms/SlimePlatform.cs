using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePlatform : MonoBehaviour
{
    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player")
        {
            if (!col.gameObject.GetComponent<BlobBert>())
                Physics.IgnoreCollision(GetComponent<Collider>(), col.gameObject.GetComponent<Collider>());
        }

    }
}
