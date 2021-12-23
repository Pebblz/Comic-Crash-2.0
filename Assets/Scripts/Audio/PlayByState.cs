using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class PlayByState : MonoBehaviour
{
    
    AudioSource src;
    private void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    public virtual void Update()
    {
        if (determine_if_play() && !is_playing())
        {
            start_playing();
        } else if (!determine_if_play())
        {
            stop_playing();
        }
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

    public abstract bool determine_if_play();
}
