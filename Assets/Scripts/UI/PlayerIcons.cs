using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class PlayerIcons : MonoBehaviour
{
    [SerializeField] Image[] Images = new Image[3];
    
    public void MoveLeft()
    {
        Sprite[] temp = new Sprite[Images.Length];
        for(int i =0; i < Images.Length; i++)
        {
            temp[i] = Images[i].sprite;
        }
        
        Images[2].sprite = temp[1];
        Images[0].sprite = temp[2];
        Images[1].sprite = temp[0];
        Debug.Log("Hello");
        

    }
    public void MoveRight()
    {

        Sprite[] temp = new Sprite[Images.Length];
        for (int i = 0; i < Images.Length; i++)
        {
            temp[i] = Images[i].sprite;
        }

        Images[2].sprite = temp[0];
        Images[0].sprite = temp[1];
        Images[1].sprite = temp[2];
        Debug.Log("Hello");

    }
}
