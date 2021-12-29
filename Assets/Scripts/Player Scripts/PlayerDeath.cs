using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerDeath : MonoBehaviour
{
    [HideInInspector, Tooltip("When this bool gets set to true it makes everything happen")]
    public bool isdead;

    [Tooltip("This is here for fading the screen when you die")]
    GameObject Fader;

    PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    void Update()
    {

        if (isdead)
        {
            GetComponent<PlayerMovement>().anim.SetBool("Dead", true);
            Death();
        }
    }
    public void Death()
    {
        if (photonView.IsMine)
        {
            GetComponent<Player>().RepoPlayer();
            if (Fader == null)
            {
                Fader = GameObject.Find("Fader");
            }
            Fader.GetComponent<Fading_Screen>().FadeOut();
            if (Fader.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Fade_Out"))
            {
                Fader.GetComponent<Fading_Screen>().FadeIn();
                GetComponent<PlayerHealth>().ResetHealth();
                GetComponent<PlayerMovement>().anim.SetBool("Dead", false);
                isdead = false;
            }
        }
    }
}
