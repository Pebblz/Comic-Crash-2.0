using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField]
    GameObject PausePage;

    GameManager Gm;

    GameObject player;

    float pauseTimer;

    public bool isPaused;
    void Start()
    {
        Gm = GetComponent<GameManager>();
        player = FindObjectOfType<PlayerMovement>().gameObject;
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
            if (Input.GetKeyDown(KeyCode.Escape) && pauseTimer <= 0)
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
        if (pause)
        {
            //Time.timeScale = 0;
            player.GetComponent<PlayerMovement>().enabled = false;
            Gm.unlockCursor();
            PausePage.SetActive(true);
        }
        else
        {
            //Time.timeScale = 1;
            player.GetComponent<PlayerMovement>().enabled = true;
            Gm.lockCursor();
            PausePage.SetActive(false);
        }
    }
}
