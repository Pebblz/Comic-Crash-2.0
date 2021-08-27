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
    public Vector2 StartPos;
    public GameObject Character;
    private void Start()
    {
        StartPos = transform.position;
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

        if(!Ontile)
            transform.position = StartPos;
    }
}
