using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayOneShot : MonoBehaviour
{
    AudioSource src;
    public float timeout = 2f;

    private void Awake()
    {
        src = this.GetComponent<AudioSource>();
        src.Play();
    }

    private void Update()
    {
        timeout -= Time.deltaTime;
        if(timeout <= 0)
        {
            Destroy(this.gameObject);
        }
    }

}
