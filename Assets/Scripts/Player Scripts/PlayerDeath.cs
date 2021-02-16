using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Tooltip("When this bool gets set to true it makes everything happen")]
    public bool isdead;

    [Tooltip("This is here for fading the screen when you die")]
    public GameObject Fader;

    void Update()
    {
        if (isdead)
        {
            Death();
        }
    }
    public void Death()
    {
        GetComponent<Player>().RepoPlayer();

        Fader.GetComponent<Fading_Screen>().FadeOut();
        if (Fader.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Fade_Out"))
        {
            Fader.GetComponent<Fading_Screen>().FadeIn();
            GetComponent<PlayerHealth>().ResetHealth();
            isdead = false;
        }

    }
}
