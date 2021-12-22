using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayState : MonoBehaviour
{
    
    AudioSource src;
    private void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    public bool is_playing()
    {
        return this.src.isPlaying;
    }

    public void start_playing()
    {
        src.loop = true;
        src.Play();
    }

    public void stop_playing()
    {
        src.Stop();
    }
}
