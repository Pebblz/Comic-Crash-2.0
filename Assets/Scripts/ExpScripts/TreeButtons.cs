using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TreeButtons : MonoBehaviour
{
    [SerializeField] Sprite inactive, active, owned;
    //the button needed to buy this one. if null it'll be buyable
    [SerializeField, Tooltip("The button that is required to unlock this upgrade. Leave it empty if it's the first upgrade in the tree")] TreeButtons ButtonReq;
    public bool Bought;
    Image img;
    Button btn;
    void Start()
    {
        img = GetComponent<Image>();
        btn = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        //if you buy the upgrade
        if (Bought)
        {
            img.sprite = owned;
            btn.interactable = false;
        }
        else
        {

            if (ButtonReq != null)
            {
                //if the previous upgrade was bought
                if (ButtonReq.Bought)
                {
                    img.sprite = active;
                    btn.interactable = true;
                }
                else
                {
                    //if the previous upgrade wasn't bought
                    img.sprite = inactive;
                    btn.interactable = false;
                }
            } else
            {
                //if this is the first upgrade in the tree
                img.sprite = active;
                btn.interactable = true;
            }
        }
    }
    public void BuyUpgrade()
    {
        if(CharacterTree.skillPoints > 0 && !Bought)
        {
            Bought = true;
            CharacterTree.skillPoints -= 1;
        }
        else
        {
            print("not enough skill points");
        }
    }
    
}
