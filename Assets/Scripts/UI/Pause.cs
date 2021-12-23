using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Luminosity.IO;
using Photon.Realtime;
using Photon.Pun;
public class Pause : MonoBehaviour
{
    [SerializeField]
    GameObject PausePage;

    public GameObject PauseFirstButton;

    GameManager Gm;

    GameObject player;

    float pauseTimer;

    public bool isPaused;

    PlayerSwitcher Ps;
    void Start()
    {
        Gm = GetComponent<GameManager>();
        //player = FindObjectOfType<PlayerMovement>().gameObject;
        Ps = GetComponent<PlayerSwitcher>();
        if (PausePage == null)
        {
            PausePage = GameObject.FindGameObjectWithTag("Pause_Menu");
            PausePage.SetActive(false);
        }
    }
    void Update()
    {
        if (player != null)
        {
            if (PausePage != null)
            {
                if (InputManager.GetButtonDown("Pause") && pauseTimer <= 0)
                {
                    isPaused = !isPaused;
                    pause(isPaused);
                    pauseTimer = .2f;
                }
            }
        }
        else
        {
            player = PhotonFindCurrentClient();
        }
        pauseTimer -= Time.unscaledDeltaTime;
    }
    GameObject PhotonFindCurrentClient()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject g in players)
        {
            if (g.GetComponent<PhotonView>().IsMine)
                return g;
        }
        return null;
    }
    public void pause(bool pause)
    {
        Ps.CanSwitch = !pause;
        if (pause)
        {
            player.GetComponent<PlayerMovement>().CantMove = true;
            player.GetComponent<PlayerMovement>().StopAllAnimations();
            Gm.unlockCursor();
            PausePage.SetActive(true);
            if (EventSystem.current != PauseFirstButton)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(PauseFirstButton);
            }


        }
        else
        {
            player.GetComponent<PlayerMovement>().CantMove = false;
            Gm.lockCursor();
            PausePage.SetActive(false);
        }
    }
}
