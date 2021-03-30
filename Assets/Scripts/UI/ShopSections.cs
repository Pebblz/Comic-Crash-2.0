using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopSections : MonoBehaviour
{
    GameManager Gm;
    public bool IsBought;
    Text text;
    [SerializeField]int cost;
    [SerializeField] WhatToGive GivenItem;
    [SerializeField, Range(1, 3)] int amountGiven;
    void Start()
    {
        text = GetComponentInChildren<Text>();
        Gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if(IsBought)
        {
            Bought();
        } else
        {
            NotBought();
        }
    }
    public void BuySection()
    {
        if(Gm.coinCount >= cost)
        {
            Gm.coinCount -= cost;
            if (GivenItem == WhatToGive.collectible)
                Gm.CollectibleCount += amountGiven;

            print(Gm.CollectibleCount);

            IsBought = true;
        }
        else
        {
            print("Not Enough Money");
        }
    }
    void NotBought()
    {
        text.text = "Cost : " + cost;
    }
    void Bought()
    {
        if(IsBought)
        {
            text.text = "Bought";
        }
        GetComponent<Button>().enabled = false;
    }
    //we will add more to here later
    enum WhatToGive
    {
        collectible
    }
}
