using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField]
    GameObject PausePage;

    GameManager Gm;

    float pauseTimer;

    bool isPaused;
    void Start()
    {
        Gm = GetComponent<GameManager>();
        PausePage = GameObject.FindGameObjectWithTag("Pause_Menu");
        PausePage.SetActive(false);
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
        }
        pauseTimer -= Time.unscaledDeltaTime;
    }
    public void pause(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
            Gm.unlockCursor();
            PausePage.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            Gm.lockCursor();
            PausePage.SetActive(false);
        }
    }
}
