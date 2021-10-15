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
    void Start()
    {
        SetTimer();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            SetTimer();
            Jump();
        }
    }
    void Jump()
    {

    }
    void SetTimer()
    {
        timer = Random.Range(MinJumpTimer, MaxJumpTimer);
    }
}
