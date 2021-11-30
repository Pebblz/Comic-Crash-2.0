using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lure : MonoBehaviour
{
    public Transform cast_point;

    Rigidbody body;
    public float distance_from_point;
    public float speed = 3f;

    public float drag_water;
    public float mass_water;

    float init_mass;
    float init_drag;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        init_mass = body.mass;
        init_drag = body.drag;
    }

    private void Update()
    {
        reel();
    }

    public void reel()
    {
        Debug.Log("Reeling");
        Vector3 dir = (cast_point.position - this.transform.position).normalized;
        this.body.AddForce(dir * speed , ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Water")
        {
            body.drag = drag_water;
            body.mass = mass_water;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            body.drag = drag_water;
            body.mass = mass_water;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            body.drag = init_drag;
            body.mass = init_mass;
        }
    }
}
