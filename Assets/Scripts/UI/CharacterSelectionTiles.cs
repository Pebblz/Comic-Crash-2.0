﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CharacterSelectionTiles : MonoBehaviour
{
    [SerializeField]
    DragableCharactersOrganizer dOrg;
    public bool OnThisTile;
    public DraggableUI CharacterOnTile;
    public DraggableUI HoveringCharacter;
    [SerializeField]
    CharacterSelectionTiles[] otherTiles = new CharacterSelectionTiles[3];
    void Update()
    {
        if(Input.GetMouseButtonUp(0) && OnThisTile)
        {
            ChangeCharacters();
        }
        
    }

    #region 2D collision
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<DraggableUI>())
        {
            OnThisTile = true;
            HoveringCharacter = col.gameObject.GetComponent<DraggableUI>();
            HoveringCharacter.Ontile = true;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    { 
        if (col.GetComponent<DraggableUI>())
        {
            if (col.GetComponent<DraggableUI>().Ontile)
            {
                OnThisTile = false;
                col.GetComponent<DraggableUI>().Ontile = false;
                HoveringCharacter = null;
            }
            if(col.gameObject == CharacterOnTile)
            {
                CharacterOnTile = null;
            }
        }
    }
    #endregion

    public void ChangeCharacters()
    {
        if (CharacterOnTile != HoveringCharacter)
        {
            if (OnThisTile)
            {
                if (CharacterOnTile == null)
                {
                    dOrg.GetRidOfUIFromList(HoveringCharacter.gameObject);
                    CharacterOnTile = HoveringCharacter;
                }
                else
                {
                    dOrg.GetRidOfUIFromList(HoveringCharacter.gameObject);
                    dOrg.AddToList(CharacterOnTile.gameObject);
                    dOrg.ReOrderList();
                    CharacterOnTile.Ontile = false;
                    CharacterOnTile = HoveringCharacter;
                }
                CharacterOnTile.transform.position = this.transform.position;
                HoveringCharacter.Ontile = true;
                HoveringCharacter = null;
                OnThisTile = false;

                foreach (CharacterSelectionTiles t in otherTiles)
                {
                    if (t.CharacterOnTile == CharacterOnTile)
                    {
                        t.CharacterOnTile = null;
                    }
                }
            }
        }

    }
}
