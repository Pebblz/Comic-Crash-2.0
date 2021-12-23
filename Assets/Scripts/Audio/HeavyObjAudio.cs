using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HeavyObjAudio : PlayByState
{
    // Start is called before the first frame update
    Rigidbody body;
    public override void Awake()
    {
        body = GetComponentInParent<Rigidbody>();
        base.Awake();

    }
    public override bool determine_if_play()
    {
        if (body.velocity.x >  0.01  || body.velocity.x < -0.01 ||
            body.velocity.z > 0.01 || body.velocity.z < -0.01)
        {
            return true;
        }
        return false;   
        
    }
}
