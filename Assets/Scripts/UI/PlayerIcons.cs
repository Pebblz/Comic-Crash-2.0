using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerIcons : MonoBehaviour
{
    [SerializeField] Image[] Images = new Image[3];
    [SerializeField] Image[] temp = new Image[3];
    public void MoveLeft()
    {
        temp = Images;

        Images[2].sprite = temp[0].sprite;
        Images[0].sprite = temp[1].sprite;
        Images[1].sprite = temp[2].sprite;

    }
    public void MoveRight()
    {

        Image[] temp = Images;
        //Icons[1].GetComponentInChildren<Image>().sprite = temp[2].GetComponentInChildren<Image>().sprite;
        //Icons[0].GetComponentInChildren<Image>().sprite = temp[1].GetComponentInChildren<Image>().sprite;
        //Icons[2].GetComponentInChildren<Image>().sprite = temp[0].GetComponentInChildren<Image>().sprite;

    }
}
