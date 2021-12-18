using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideWithHeavyBlock : MonoBehaviour
{
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "HeavyObject")
            Physics.IgnoreCollision(col.collider, gameObject.GetComponent<Collider>());
    }
}
