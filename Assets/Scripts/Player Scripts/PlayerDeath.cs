using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public bool isdead;
    public GameObject Fader;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
