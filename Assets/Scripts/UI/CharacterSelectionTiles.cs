using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CharacterSelectionTiles : MonoBehaviour
{
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
    public void ChangeCharacters()
    {
        if (CharacterOnTile != HoveringCharacter)
        {
            if (OnThisTile)
            {

                if (CharacterOnTile == null)
                {
                    CharacterOnTile = HoveringCharacter;
                }
                else
                {
                    CharacterOnTile.Ontile = false;
                    CharacterOnTile.StartPos = HoveringCharacter.StartPos;
                    CharacterOnTile.transform.position = CharacterOnTile.StartPos;
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
