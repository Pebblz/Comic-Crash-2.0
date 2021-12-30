using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIcons : MonoBehaviour
{
    [SerializeField] GameObject[] Icons = new GameObject[3];
    [SerializeField] Transform Main;
    Vector3 MainScale;
    Vector3 SmallScale;
    [SerializeField] Transform Small;
    [SerializeField] Transform[] iconPositions = new Transform[3];
    [SerializeField] Transform[] iconCurrentPositions = new Transform[3];

    int MiddleCurrentIndex;
    private void Start()
    {
        MainScale = Main.localScale;
        SmallScale = Small.localScale;
    }
    public void MoveLeft()
    {
        //Transform[] temp = iconCurrentPositions;
        //if(MiddleCurrentIndex == 0)
        //{
        //    MiddleCurrentIndex = 2;
        //}
        //else
        //    MiddleCurrentIndex--;

        
        //iconCurrentPositions[0] = temp[2];

        //iconCurrentPositions[1] = temp[0];

        //iconCurrentPositions[2] = temp[1];

        //Icons[2].transform.position = iconPositions[1].position;
        //Icons[2].transform.localScale = SmallScale;

        //Icons[1].transform.position = iconPositions[0].position;
        //Icons[1].transform.localScale = MainScale;


        //Icons[0].transform.position = iconPositions[2].position;
        //Icons[0].transform.localScale = SmallScale;

    }
    public void MoveRight()
    {
        //Transform[] temp = iconCurrentPositions;
        //if(MiddleCurrentIndex == 2)
        //{
        //    MiddleCurrentIndex = 0;
        //}
        //else
        //    MiddleCurrentIndex++;
        //iconCurrentPositions[2] = temp[0];
        //iconCurrentPositions[1] = temp[2];
        //iconCurrentPositions[0] = temp[1];

        //Icons[0].transform.position = iconPositions[1].position;
        //Icons[0].transform.localScale = MainScale;

        //Icons[2].transform.position = iconPositions[0].position;
        //Icons[2].transform.localScale = SmallScale;

        //Icons[1].transform.position = iconPositions[2].position;
        //Icons[1].transform.localScale = MainScale;

    }
}
