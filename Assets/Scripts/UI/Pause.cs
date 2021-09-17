using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class Pause : MonoBehaviour
{
    [SerializeField]
    GameObject PausePage;

    GameManager Gm;

    GameObject player;

    float pauseTimer;

    public bool isPaused;

    PlayerSwitcher Ps;
    void Start()
    {
        Gm = GetComponent<GameManager>();
        player = FindObjectOfType<PlayerMovement>().gameObject;
        Ps = GetComponent<PlayerSwitcher>();
        if (PausePage == null)
        {
            PausePage = GameObject.FindGameObjectWithTag("Pause_Menu");
            PausePage.SetActive(false);
        }
    }
    void Update()
    {
        if (PausePage != null)
        {
            if (InputManager.GetButtonDown("Pause") && pauseTimer <= 0)
            {
                isPaused = !isPaused;
                pause(isPaused);
                pauseTimer = .2f;
            }
            if(player == null)
            {
                player = FindObjectOfType<PlayerMovement>().gameObject;
            }
        }
        pauseTimer -= Time.unscaledDeltaTime;
    }
    public void pause(bool pause)
    {
        Ps.CanSwitch = !pause;
        if (pause)
        {
            player.GetComponent<PlayerMovement>().CantMove = true;
            Gm.unlockCursor();
            PausePage.SetActive(true);
        }
        else
        {
            player.GetComponent<PlayerMovement>().CantMove = false;
            Gm.lockCursor();
            PausePage.SetActive(false);
        }
    }
}
