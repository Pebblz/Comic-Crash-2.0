using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelTrigger : MonoBehaviour
{
    [SerializeField, Range(1,5)] int LevelNumber;
    [SerializeField] Text TopText;
    [SerializeField] GameObject AskerPanel;
    [SerializeField, Range(0, 150)] int CollectiblesRequired;
    GameManager gameManager;
    [SerializeField] TextMesh textMesh;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    void Update()
    {
        if(gameManager.CollectibleCount < CollectiblesRequired)
        {
            textMesh.text = "You still need " + (CollectiblesRequired -= gameManager.CollectibleCount).ToString() + " collectibles left";
        } else if (gameManager.CollectibleCount >= CollectiblesRequired)
        {
            textMesh.text = "";
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player" && gameManager.CollectibleCount >= CollectiblesRequired)
        {
            OpenAsker();
        }
    }
    void OpenAsker()
    {
        AskerPanel.SetActive(true);
        gameManager.unlockCursor();
        if(TopText.text == "")
            TopText.text = "Would you like to go to level " + LevelNumber + " ?"; 
    }
    public void CloseAsker()
    {
        AskerPanel.SetActive(false);
        gameManager.lockCursor();
    }
}
