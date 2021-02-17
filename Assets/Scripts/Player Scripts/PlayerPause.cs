using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPause : MonoBehaviour
{
    [SerializeField]
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
        Player.GetComponent<PlayerMovement>().enabled = false;
        gameManager.GetComponent<PlayerSwitcher>().enabled = false;
        gameManager.GetComponent<GameManager>().unlockCursor();
    }
    void UnPaused()
    {
        Player.GetComponent<PlayerMovement>().enabled = true;
        gameManager.GetComponent<PlayerSwitcher>().enabled = true;
        gameManager.GetComponent<GameManager>().lockCursor();
    }
}
