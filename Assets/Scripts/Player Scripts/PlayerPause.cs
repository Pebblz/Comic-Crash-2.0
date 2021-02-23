using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPause : MonoBehaviour
{
    
    bool IsPaused = false;
    float PauseTimer;
    private GameObject Player;
    GameObject gameManager;
    void Awake()
    {
        gameManager = GameObject.Find("GameManager");
        Player = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (!IsPaused && PauseTimer < 0)
            {
                IsPaused = true;
                Paused();
                PauseTimer = 1;
            }
            if (IsPaused && PauseTimer < 0)
            {
                IsPaused = false;
                UnPaused();
                PauseTimer = 1;
            }
        }

        PauseTimer -= Time.deltaTime;
    }
    void Paused()
    {
        //this stops all things that rely on time to do anything
        Time.timeScale = 0;
        //this stops the players movement
        Player.GetComponent<PlayerMovement>().enabled = false;
        //this stops the player from switching characters
        gameManager.GetComponent<PlayerSwitcher>().enabled = false;
        //and unlocks your cursor so you can use the pause menu
        gameManager.GetComponent<GameManager>().unlockCursor();
    }
    void UnPaused()
    {
        //this resumes all things that rely on time
        Time.timeScale = 1;
        Player.GetComponent<PlayerMovement>().enabled = true;
        gameManager.GetComponent<PlayerSwitcher>().enabled = true;
        gameManager.GetComponent<GameManager>().lockCursor();
    }
}
