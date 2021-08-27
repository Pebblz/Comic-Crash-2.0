using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitcherMachine : MonoBehaviour
{
    [SerializeField]
    GameObject CharacterSwitcherUI;

    GameObject Player;

    [SerializeField, Range(.1f, 20f)]
    float Range;
    private void Update()
    {
        if (Player == null)
            Player = FindObjectOfType<PlayerMovement>().gameObject;

        if(InRadius())
        {
            if (Input.GetKeyDown(KeyCode.E))
                CharacterSwitcherUI.SetActive(!CharacterSwitcherUI.activeSelf);
        }

    }

    bool InRadius()
    {
        if(Vector3.Distance(gameObject.transform.position, Player.transform.position) < Range)
        {
            return true;
        }


        return false;
    }
}
