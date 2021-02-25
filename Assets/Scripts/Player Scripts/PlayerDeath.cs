using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Tooltip("When this bool gets set to true it makes everything happen")]
    [HideInInspector]
    public bool isdead;

    [Tooltip("This is here for fading the screen when you die")]
    GameObject Fader;
    private void Awake()
    {
        Fader = GameObject.Find("Fader");
    }
    void Update()
    {

        if (isdead)
        {
            GetComponent<Animator>().SetBool("Dead", true);
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
            GetComponent<Animator>().SetBool("Dead", false);
            isdead = false;
        }

    }
}
