using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWork : MonoBehaviour
{
    [SerializeField] AudioClip fireworkSizzle;
    [SerializeField] AudioClip fireworkExplosion;

    int numberOfParticles;
    void Start()
    {
        //play sizzle sound effect
        numberOfParticles = GetComponent<ParticleSystem>().particleCount;
    }

    // Update is called once per frame
    void Update()
    {
        if(!fireworkSizzle && !fireworkExplosion)
        {
            return;
        }
        var count = GetComponent<ParticleSystem>().particleCount;

        if(count < numberOfParticles)
        {
            //play explosion
            numberOfParticles = count;
        }
    }
}
