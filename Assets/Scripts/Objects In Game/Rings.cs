using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rings : MonoBehaviour
{
    public bool Hit;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
            Hit = true;
    }
}
