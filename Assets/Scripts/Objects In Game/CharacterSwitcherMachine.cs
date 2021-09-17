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

    PlayerSwitcher Ps;

    CharacterSwitcherMachine[] otherMachines;

    private void Start()
    {
        camera = FindObjectOfType<MainCamera>().gameObject;
        Ps = FindObjectOfType<PlayerSwitcher>();
        otherMachines = FindObjectsOfType<CharacterSwitcherMachine>();
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
                if (CharacterSwitcherUI.activeSelf && !CheckIfAnyMachineIsInRange())
                {
                    EText.SetActive(false);
                }
                if (Input.GetButtonDown("Interact"))
                {
                    CharacterSwitcherUI.SetActive(!CharacterSwitcherUI.activeSelf);

                    if (CharacterSwitcherUI.activeSelf)
                    {
                        Ps.CanSwitch = false;
                        player.GetComponent<PlayerMovement>().CantMove = true;
                        camera.GetComponent<MainCamera>().StopCamera = true;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                    else
                    {
                        Ps.CanSwitch = true;
                        player.GetComponent<PlayerMovement>().CantMove = false;
                        camera.GetComponent<MainCamera>().StopCamera = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
            }
            else
            {

                if (CharacterSwitcherUI.activeSelf)
                {
                    if (!CheckIfAnyMachineIsInRange())
                    {
                        Ps.CanSwitch = true;
                        //this is here just incase the player finds a way to die while in the switcher menu
                        player.GetComponent<PlayerMovement>().CantMove = false;
                        camera.GetComponent<MainCamera>().StopCamera = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        CharacterSwitcherUI.SetActive(false);
                    }
                }
                if (EText.activeSelf)
                {
                    if (!CheckIfAnyMachineIsInRange())
                        EText.SetActive(false);
                }
            }
        }
    }
    public void CloseSwitcher()
    {
        Ps.CanSwitch = true;
        CharacterSwitcherUI.SetActive(false);
        player.GetComponent<PlayerMovement>().CantMove = false;
        camera.GetComponent<MainCamera>().StopCamera = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    bool CheckIfAnyMachineIsInRange()
    {
        foreach (CharacterSwitcherMachine c in otherMachines)
        {
            if (c.InRadius())
            {
                return true;
            }
        }
        return false;
    }
    public bool InRadius()
    {
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) < Range)
        {
            return true;
        }


        return false;
    }
}
