using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitcherMachine : MonoBehaviour
{
    [SerializeField]
    GameObject CharacterSwitcherUI;

    [SerializeField]
    GameObject EText;

    GameObject player;

    [SerializeField, Range(.1f, 20f)]
    float Range;

    GameObject camera;
    private void Start()
    {
        camera = FindObjectOfType<MainCamera>().gameObject;
    }
    private void Update()
    {
        if (player == null)
            player = FindObjectOfType<PlayerMovement>().gameObject;
        else
        {
            if (InRadius())
            {
                if (!CharacterSwitcherUI.activeSelf)
                {
                    EText.SetActive(true);
                }
                else
                {
                    EText.SetActive(false);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    CharacterSwitcherUI.SetActive(!CharacterSwitcherUI.activeSelf);

                    if (CharacterSwitcherUI.activeSelf)
                    {
                        player.GetComponent<PlayerMovement>().CantMove = true;
                        camera.GetComponent<MainCamera>().StopCamera = true;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                    else
                    {
                        player.GetComponent<PlayerMovement>().CantMove = false;
                        camera.GetComponent<MainCamera>().StopCamera = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
            }
            else
            {
                //this is here just incase the player finds a way to die while in the switcher menu
                player.GetComponent<PlayerMovement>().CantMove = false;
                camera.GetComponent<MainCamera>().StopCamera = false;
                CharacterSwitcherUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                EText.SetActive(false);
            }
        }
    }
    public void CloseSwitcher()
    {
        CharacterSwitcherUI.SetActive(false);
        player.GetComponent<PlayerMovement>().CantMove = false;
        camera.GetComponent<MainCamera>().StopCamera = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    bool InRadius()
    {
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) < Range)
        {
            return true;
        }


        return false;
    }
}
