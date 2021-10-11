using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float acceleration = 10f, speed = 10f;
    [SerializeField]
    int damage;
    private void OnCollisionEnter(Collision col)
    {
        //blobBert shouldn't be able to jump on the slimeoline
        if (col.gameObject.tag == "Player")
        {
            Rigidbody body = col.gameObject.GetComponent<Rigidbody>();
            col.gameObject.GetComponent<PlayerHealth>().HurtPlayer(damage);
            if (body)
            {
                col.gameObject.GetComponent<PlayerMovement>().Bounce = true;
                col.gameObject.GetComponent<PlayerMovement>().PlayJumpAnimation();
                Accelerate(body);
            }
        }
    }
    void Accelerate(Rigidbody body)
    {
        Vector3 velocity = transform.InverseTransformDirection(body.velocity);
        if (acceleration > 0f)
        {
            velocity.y = Mathf.MoveTowards(velocity.y, speed, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.y = speed;
        }
        body.velocity = transform.TransformDirection(velocity);
        if (body.TryGetComponent(out PlayerMovement player))
        {
            player.PreventSnapToGround();
        }
    }
}
