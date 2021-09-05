using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
        Gm = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerMovement>().gameObject;
        Ps = FindObjectOfType<PlayerSwitcher>();
        if (PausePage == null)
        {
            PausePage = GameObject.FindGameObjectWithTag("Pause_Menu");
            PausePage.SetActive(false);
        }
    }
    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>().gameObject;
        }
        pauseTimer -= Time.unscaledDeltaTime;
    }
    public void pause(InputAction.CallbackContext context)
    {
        if (pauseTimer < 0 && context.ReadValue<float>() > 0)
        {
            isPaused = !isPaused;
            Ps.CanSwitch = !isPaused;
            if (isPaused)
            {
                player.GetComponent<PlayerMovement>().enabled = false;
                Gm.unlockCursor();
                PausePage.SetActive(true);
            }
            else
            {
                player.GetComponent<PlayerMovement>().enabled = true;
                Gm.lockCursor();
                PausePage.SetActive(false);
            }
            pauseTimer = .2f;
        }
    }
    public void UnPause()
    {
        player.GetComponent<PlayerMovement>().enabled = true;
        Gm.lockCursor();
        PausePage.SetActive(false);
        pauseTimer = 0;
        isPaused = false;
        Ps.CanSwitch = true;
    }
}
