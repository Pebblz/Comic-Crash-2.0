using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Luminosity.IO;
using Luminosity.IO.Examples;
public class DraggableUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool dragging;
    public bool Ontile;
    private Vector2 offset;
    public GameObject Character;
    public GameObject TileOn;
    DragableCharactersOrganizer Dorg;
    GamepadToggle gm;
    Button button;
    private void Start()
    {
        gm = FindObjectOfType<GamepadToggle>();
        button = GetComponent<Button>();
        Dorg = FindObjectOfType<DragableCharactersOrganizer>();
    }
    public void Update()
    {

        if (gm.m_gamepadOn)
        {
            button.enabled = true;
            if (EventSystem.current.currentSelectedGameObject == gameObject)
            {
                if (InputManager.GetButtonDown("UI_Submit") && !dragging)
                {
                    dragging = true;
                } 
            }
            if (InputManager.GetButtonDown("UI_Submit") && dragging)
            {
                if (EventSystem.current.currentSelectedGameObject.GetComponent<CharacterSelectionTiles>())
                {
                    if (Dorg.CheckIfObjectIsInList(gameObject))
                    {
                        Dorg.ReOrderList();
                    }
                    else
                    {

                        Dorg.AddToList(gameObject);
                        Dorg.ReOrderList();
                    }
                    if (TileOn != null)
                    {
                        if (TileOn.GetComponent<CharacterSelectionTiles>().CharacterOnTile == gameObject.GetComponent<DraggableUI>())
                            TileOn.GetComponent<CharacterSelectionTiles>().CharacterOnTile = null;
                    }
                    print(TileOn);
                    dragging = false;
                }
                if (EventSystem.current.gameObject.GetComponent<DraggableUI>())
                {
                    dragging = false;
                }
            }
        }
        else
        {
            button.enabled = false;
            if (dragging)
            {
                transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - offset;
            }
        }
    }

    #region Pointer Methods
    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;

        if (!Ontile)
        {
            if (TileOn != null)
            {
                if (TileOn.GetComponent<CharacterSelectionTiles>().CharacterOnTile == gameObject.GetComponent<DraggableUI>())
                    TileOn.GetComponent<CharacterSelectionTiles>().CharacterOnTile = null;
            }
            if (Dorg.CheckIfObjectIsInList(gameObject))
            {
                Dorg.ReOrderList();
            } else 
            {

                Dorg.AddToList(gameObject);
                Dorg.ReOrderList();
            }
        }
    }
    #endregion
}
