using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragableCharactersOrganizer : MonoBehaviour
{
    [SerializeField]
    List<GameObject> dragableUI;
    Vector2[] dragableUIPositions;

    private void Start()
    {
        dragableUIPositions = new Vector2[dragableUI.Count];

        for (int i = 0; i < dragableUI.Count; i++)
        {
            dragableUIPositions[i] = dragableUI[i].transform.position;
        }
    }

    #region Helper Functions
    //this here is to make everything nice and neat
    public void ReOrderList()
    {
        for (int i = 0; i < dragableUI.Count; i++)
        {
            dragableUI[i].transform.position = dragableUIPositions[i];
        }
    }
    public void GetRidOfUIFromList(GameObject UI)
    {
        for (int i = 0; i < dragableUI.Count; i++)
        {
            if (dragableUI[i] == UI)
                dragableUI.RemoveAt(i);
        }
    }
    public void AddToList(GameObject UI)
    {
        if (!dragableUI.Contains(UI))
            dragableUI.Add(UI);
    }
    public bool CheckIfObjectIsInList(GameObject UI)
    {
        if (dragableUI.Contains(UI))
            return true;
        else
            return false;
    }
    #endregion
}
