using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    Pause pause;
    public GameObject PauseScreen, controlBindsMenu, ControllerBindMenu, KeyboardBindMenu;

    //all these are to navigate the pause menu with controller. the closed buttons are for 
    //when you close that tag it'll put the xbox navigation over the button to go to the menu
    //just closed 
    [Header ("For xbox navigation")]
    public GameObject BindingFirstButton, KeyBoardBindingFirstButton,
        ControllerBindingFirstButton, bindingClosedButton, KetBoardClosedButton, 
        ControllerClosedButton;


    void Start()
    {
        pause = FindObjectOfType<Pause>();
    }
    private void Update()
    {
        if (!pause.isPaused && controlBindsMenu.activeSelf ||
            !pause.isPaused && ControllerBindMenu.activeSelf ||
            !pause.isPaused && KeyboardBindMenu.activeSelf)
        {
            controlBindsMenu.SetActive(false);
            ControllerBindMenu.SetActive(false);
            KeyboardBindMenu.SetActive(false);
        }
    }

    public void CloseMenuBTN()
    {
        pause.pause(false);
        pause.isPaused = false;
    }
    public void OpenControlBinds()
    {
        PauseScreen.SetActive(false);
        controlBindsMenu.SetActive(true);

        SetEventSystem(BindingFirstButton);
    }
    public void ControlBindBackButton()
    {
        PauseScreen.SetActive(true);
        controlBindsMenu.SetActive(false);

        SetEventSystem(bindingClosedButton);
    }
    public void EditKeyBoardControls()
    {
        controlBindsMenu.SetActive(false);
        KeyboardBindMenu.SetActive(true);

        SetEventSystem(KeyBoardBindingFirstButton);
    }
    public void KeyBoardBackBTN()
    {
        controlBindsMenu.SetActive(true);
        KeyboardBindMenu.SetActive(false);

        SetEventSystem(KetBoardClosedButton);
    }
    public void EditControllerBinds()
    {
        controlBindsMenu.SetActive(false);
        ControllerBindMenu.SetActive(true);

        SetEventSystem(ControllerBindingFirstButton);
    }
    public void ControllerBackBTN()
    {
        controlBindsMenu.SetActive(true);
        ControllerBindMenu.SetActive(false);

        SetEventSystem(ControllerClosedButton);
    }
    //this is here just to shorten the code
    void SetEventSystem(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}
