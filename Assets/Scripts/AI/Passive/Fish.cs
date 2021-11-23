using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{

    enum STATE
    {
        IDLE,
        CAUGHT,
        ON_THE_HOOK,
        RESPAWN
    }

    public float stamina;
    private float time_on_hook = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
