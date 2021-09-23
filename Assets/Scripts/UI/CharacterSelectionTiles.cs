using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Luminosity.IO;
using Luminosity.IO.Examples;
public class CharacterSelectionTiles : MonoBehaviour
{
    [SerializeField]
    DragableCharactersOrganizer dOrg;
    public bool OnThisTile;
    public DraggableUI CharacterOnTile;
    public DraggableUI HoveringCharacter;
    [SerializeField]
    CharacterSelectionTiles[] otherTiles = new CharacterSelectionTiles[3];
    bool once;
    GamepadToggle gm;
    Button button;
    private void Start()
    {
        gm = FindObjectOfType<GamepadToggle>();
        button = GetComponent<Button>();
    }
    void Update()
    {
        if (gm.m_gamepadOn)
        {
            button.enabled = true;
            if (EventSystem.current.currentSelectedGameObject == this.gameObject && once)
            {
                DraggableUI[] ui = FindObjectsOfType<DraggableUI>();

                foreach (DraggableUI i in ui)
                {
                    if (i.dragging)
                    {
                        OnThisTile = true;
                        HoveringCharacter = i;
                        HoveringCharacter.Ontile = true;
                    }
                }
                once = false;
            }
            if (EventSystem.current.currentSelectedGameObject != this.gameObject && !once)
            {
                once = true;
                if (HoveringCharacter != null)
                {
                    if (HoveringCharacter.gameObject == CharacterOnTile)
                    {
                        CharacterOnTile = null;
                    }
                    OnThisTile = false;
                    HoveringCharacter.Ontile = false;
                    HoveringCharacter = null;
                }
                
            }
            if (InputManager.GetButtonUp("UI_Submit") && OnThisTile)
            {
                ChangeCharacters();
            }
            if (InputManager.GetButtonUp("UI_Submit") && CharacterOnTile != null)
            {
                CharacterOnTile.transform.position = this.transform.position;
            }
        }
        else
        {
            if (InputManager.GetMouseButtonUp(0) && OnThisTile)
            {
                ChangeCharacters();
            }
            if (InputManager.GetMouseButtonUp(0) && CharacterOnTile != null)
            {
                CharacterOnTile.transform.position = this.transform.position;
            }
            button.enabled = false;
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
            if (col.gameObject == CharacterOnTile)
            {
                CharacterOnTile = null;
            }
        }
    }
    #endregion
    public void GetRidOfCharacterTile()
    {
        if (CharacterOnTile != null)
        {
            dOrg.AddToList(CharacterOnTile.gameObject);
            dOrg.ReOrderList();
            CharacterOnTile.GetComponent<DraggableUI>().TileOn = null;
            HoveringCharacter = null;
            OnThisTile = false;
            CharacterOnTile = null;
        }
    }
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
                    CharacterOnTile.GetComponent<DraggableUI>().TileOn = gameObject;
                }
                else
                {
                    dOrg.GetRidOfUIFromList(HoveringCharacter.gameObject);
                    dOrg.AddToList(CharacterOnTile.gameObject);
                    CharacterOnTile.Ontile = false;
                    CharacterOnTile.GetComponent<DraggableUI>().TileOn = null;
                    CharacterOnTile = HoveringCharacter;
                    CharacterOnTile.GetComponent<DraggableUI>().TileOn = gameObject;
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
                dOrg.ReOrderList();
                return;
            }
        }
        else
        {
            CharacterOnTile.transform.position = this.transform.position;
            OnThisTile = false;

            if (HoveringCharacter != null)
            {
                HoveringCharacter.Ontile = true;
                HoveringCharacter = null;
            }
        }
    }
}
