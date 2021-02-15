using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    float pushPower = 5.0f;
    Vector3 force;
    Vector3 startpos;
    // Start is called before the first frame update
    void Start()
    {
        startpos = this.gameObject.transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            Vector3 pushdir = new Vector3(col.GetComponent<Rigidbody>().velocity.x, 0,
                col.GetComponent<Rigidbody>().velocity.x);

            rb.velocity = pushdir * pushPower;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Players")
        {
            force = Vector3.zero;
        }
        if (col.gameObject.name == "Out_Of_Bounds")
        {
            this.gameObject.transform.position = startpos;
            rb.velocity = new Vector3(0f, 0f, 0f);
        }
    }
}
