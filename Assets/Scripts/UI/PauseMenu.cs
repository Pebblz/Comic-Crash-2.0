using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    Pause pause;
    public GameObject PauseScreen, controlBindsMenu, ControllerBindMenu,
        KeyboardBindMenu;
    void Start()
    {
        pause = FindObjectOfType<Pause>();
    }

    public void CloseMenuBTN()
    {
        pause.pause(false);
    }
    public void OpenControlBinds()
    {
        PauseScreen.SetActive(false);
        controlBindsMenu.SetActive(true);
    }
    public void ControlBindBackButton()
    {
        PauseScreen.SetActive(true);
        controlBindsMenu.SetActive(false);
    }
    public void EditKeyBoardControls()
    {
        controlBindsMenu.SetActive(false);
        KeyboardBindMenu.SetActive(true);
    }
    public void KeyBoardBackBTN()
    {
        controlBindsMenu.SetActive(true);
        KeyboardBindMenu.SetActive(false);
    }
    public void EditControllerBinds()
    {
        controlBindsMenu.SetActive(false);
        ControllerBindMenu.SetActive(true);
    }
    public void ControllerBackBTN()
    {
        controlBindsMenu.SetActive(true);
        ControllerBindMenu.SetActive(false);
    }
}
