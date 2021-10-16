using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaBubble : MonoBehaviour
{
    [SerializeField, Tooltip("The speed at which the lava nunnle jumps up")]
    float JumpSpeed;

    [SerializeField, Tooltip("How high the lava bubble jumps")]
    float JumpHeight;

    [SerializeField]
    float MaxJumpTimer, MinJumpTimer;

    float timer;

    Rigidbody body;

    Vector3 startPos;

    [SerializeField]
    GameObject parent;

    [SerializeField]
    int damage;
    void Start()
    {
        startPos = transform.position;
        SetTimer();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            Jump();
            SetTimer();
        }
    }
    void Jump()
    {
        body.velocity = new Vector3(0, JumpHeight * JumpSpeed, 0);
    }
    void SetTimer()
    {
        timer = Random.Range(MinJumpTimer, MaxJumpTimer);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != parent)
        {
            if(collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerHealth>().HurtPlayer(damage);
            }
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());

        }
        
    }
}
