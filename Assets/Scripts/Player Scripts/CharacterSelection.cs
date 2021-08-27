using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    PlayerSwitcher Ps;

    public CharacterSelectionTiles[] Tiles = new CharacterSelectionTiles[4];
    private void Start()
    {
        Ps = FindObjectOfType<PlayerSwitcher>();
    }
    public void SwitchCharacters()
    {
        for(int i = 0; i < Tiles.Length;i++)
        {
           Ps.ChangeSelectedCharacters(i, Tiles[i].CharacterOnTile.Character);
        }
    }
}
