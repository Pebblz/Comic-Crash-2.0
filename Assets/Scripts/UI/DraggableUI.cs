using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool dragging;
    public bool Ontile;
    private Vector2 offset;
    public GameObject Character;
    DragableCharactersOrganizer Dorg;
    private void Start()
    {
        Dorg = FindObjectOfType<DragableCharactersOrganizer>();
    }
    public void Update()
    {
        if (dragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - offset;
        }
    }

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
            if(Dorg.CheckIfObjectIsInList(gameObject))
            {
                Dorg.ReOrderList();
            } else 
            {

                Dorg.AddToList(gameObject);
                Dorg.ReOrderList();
            }
        }
    }
}
