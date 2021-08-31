using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    PlayerSwitcher Ps;
    CharacterSwitcherMachine Cs;
    public CharacterSelectionTiles[] Tiles = new CharacterSelectionTiles[4];
    private void Start()
    {
        Ps = FindObjectOfType<PlayerSwitcher>();
        Cs = FindObjectOfType<CharacterSwitcherMachine>();
    }
    public void Exit_BTN()
    {
        for (int i = 0; i < Tiles.Length; i++)
        {
            if (Tiles[i].CharacterOnTile != null)
            {
                Tiles[i].GetRidOfCharacterTile();
            }
        }
        Cs.CloseSwitcher();
    }
    public void SwitchCharacters()
    {
        for(int i = 0; i < Tiles.Length;i++)
        {
            if (Tiles[i].CharacterOnTile != null)
            {
                Ps.ChangeSelectedCharacters(i, Tiles[i].CharacterOnTile.Character);
                Tiles[i].GetRidOfCharacterTile();
            }
            else
                Ps.ChangeSelectedCharacters(i, null);
        }
        Cs.CloseSwitcher();
        Ps.SwitchToFirstCharacter();
    }
}
