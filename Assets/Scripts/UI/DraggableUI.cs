using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool dragging;
    public bool Ontile;
    private Vector2 offset;
    public GameObject Character;
    public GameObject TileOn;
    DragableCharactersOrganizer Dorg;
    Camera mainCam;
    private void Start()
    {
        Dorg = FindObjectOfType<DragableCharactersOrganizer>();
        mainCam = Camera.main;
    }
    public void Update()
    {
        if (dragging)
        {
            transform.position = Mouse.current.position.ReadValue();
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
