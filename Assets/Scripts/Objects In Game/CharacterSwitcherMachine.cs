using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Luminosity.IO;
using Photon.Realtime;
using Photon.Pun;
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

    [SerializeField] GameObject FirstButton;

    private void Start()
    {
        camera = FindObjectOfType<MainCamera>().gameObject;
        Ps = FindObjectOfType<PlayerSwitcher>();
        otherMachines = FindObjectsOfType<CharacterSwitcherMachine>();
    }
    private void Update()
    {
        if (player != null)
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
                if (InputManager.GetButtonDown("Interact"))
                {
                    CharacterSwitcherUI.SetActive(!CharacterSwitcherUI.activeSelf);

                    if (CharacterSwitcherUI.activeSelf)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.SetSelectedGameObject(FirstButton);

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
        else
        {

            player = PhotonFindCurrentClient();
        }
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
