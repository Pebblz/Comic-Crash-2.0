using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class ShopSections : MonoBehaviour
{
    GameManager Gm;
    public bool IsBought;
    Text text;
    [SerializeField]int cost;
    [SerializeField] WhatToGive GivenItem;
    [SerializeField, Range(1, 3)] int amountGiven;
    [SerializeField] int BuyCount = 0;
    int TimesBought;
    void Start()
    {
        text = GetComponentInChildren<Text>();
        Gm = FindObjectOfType<GameManager>();
        GetComponent<Button>().onClick.AddListener(delegate { BuySection(); });
    }

    void Update()
    {
        if(IsBought)
        {
            Bought();
        }
        //else
        //{
        //    NotBought();
        //}
    }
    public void BuySection()
    {
        if(Gm.coinCount >= cost)
        {
            if (GivenItem == WhatToGive.collectible)
            {
                Gm.UpdateCoinCount(-cost);
                Gm.CollectibleCount += amountGiven;
                IsBought = true;
            }
            if(GivenItem == WhatToGive.Health)
            {
                PlayerHealth player = PhotonFindCurrentClient().GetComponent<PlayerHealth>();
                if(!player.AtMaxHealth())
                {
                    Gm.UpdateCoinCount(-cost);
                    player.currentHealth++;
                    IsBought = true;
                }
                else
                {
                    print(player.gameObject.name + " At full Health");
                }
            }

        }
        else
        {
            print("Not Enough Money");
        }
    }
    //void NotBought()
    //{
    //    text.text = "Cost : " + cost;
    //}
    void Bought()
    {
        if (TimesBought >= BuyCount)
        {
            if (IsBought)
            {
                text.text = "Bought";
            }
            GetComponent<Button>().enabled = false;
        }
        else
        {
            TimesBought++;
        }
    }
    GameObject PhotonFindCurrentClient()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject g in players)
        {
            if (g.GetComponent<PhotonView>().IsMine)
                return g;
        }
        return null;
    }
    enum WhatToGive
    {
        collectible,
        Health
    }
}
