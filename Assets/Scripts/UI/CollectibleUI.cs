using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleUI : MonoBehaviour
{
    [SerializeField]
    GameObject LeftUI, RightUI, BottomUI;

    Vector3 StartingLeftUIPos, StartingRightUIPos, StartingBottomUIPos;
    
    [SerializeField]
    float EndingLeftUIPosX, EndingRightUIPosX, EndingBottomUIPosY;
    [SerializeField]
    float maxSpeedX, maxSpeedY, decelartion;
    [SerializeField]
    float MaxResetTimer;
    float resetTimer;
    float stopMovement;
    [SerializeField]
    bool StartMoving;
    void Start()
    {
        stopMovement = 1f;
        StartingLeftUIPos = LeftUI.transform.position;
        StartingRightUIPos = RightUI.transform.position;
        StartingBottomUIPos = BottomUI.transform.position;
        ResetUI();
    }

    // Update is called once per frame
    void Update()
    {
        if(StartMoving)
        {
            stopMovement -= Time.deltaTime;
            if (stopMovement >= 0)
            {
                if (decelartion > .1f)
                {
                    decelartion -= Time.deltaTime;
                }
                if (LeftUI.transform.position.x > EndingLeftUIPosX)
                {
                    LeftUI.transform.position += new Vector3(maxSpeedX * (decelartion), 0, 0);
                }
                if (RightUI.transform.position.x > EndingRightUIPosX)
                {
                    RightUI.transform.position -= new Vector3(maxSpeedX * (decelartion), 0, 0);
                }
                if (BottomUI.transform.position.y > EndingBottomUIPosY)
                {
                    BottomUI.transform.position += new Vector3(0, maxSpeedY * (decelartion), 0);
                }
            }
            if (resetTimer <= 0)
                ResetUI();

            resetTimer -= Time.deltaTime;

        }
    }
    public void GotCollectible()
    {
        StartMoving = true;
        LeftUI.SetActive(true);
        RightUI.SetActive(true);
        BottomUI.SetActive(true);
    }
    public void ResetUI()
    {
        StartMoving = false;
        resetTimer = MaxResetTimer;
        stopMovement = 1f;
        LeftUI.transform.position = StartingLeftUIPos;
        RightUI.transform.position = StartingRightUIPos;
        BottomUI.transform.position = StartingBottomUIPos;
        LeftUI.SetActive(false);
        RightUI.SetActive(false);
        BottomUI.SetActive(false);
    }
}
