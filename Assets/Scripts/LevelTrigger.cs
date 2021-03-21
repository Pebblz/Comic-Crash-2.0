using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelTrigger : MonoBehaviour
{
    [SerializeField, Range(1,5)] int LevelNumber;
    [SerializeField] Text TopText;
    [SerializeField] GameObject AskerPanel;
    GameManager gameManager;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            OpenAsker();
        }
    }
    void OpenAsker()
    {
        AskerPanel.SetActive(true);
        gameManager.unlockCursor();
        TopText.text = "Would you like to go to level " + LevelNumber + " ?"; 
    }
    public void CloseAsker()
    {
        AskerPanel.SetActive(false);
        gameManager.lockCursor();
    }
}
