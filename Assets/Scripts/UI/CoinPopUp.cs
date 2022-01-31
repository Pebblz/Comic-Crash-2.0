using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CoinPopUp : MonoBehaviour
{   
    Text CoinUI;
    [SerializeField] float TimeToFade = 5;
    CanvasGroup cg;
    bool Active;
    float TimerTillFade;
    GameObject Player;
    private void Start()
    {
        CoinUI = GameObject.Find("PopUPCoin").GetComponent<Text>();
        cg = CoinUI.gameObject.GetComponent<CanvasGroup>();    
    }

    private void Update()
    {
        if (Player != null)
        {
            if (Active)
            {
                TimerTillFade -= Time.fixedDeltaTime;

                if (TimerTillFade < 0)
                {
                    cg.alpha -= Time.fixedDeltaTime;
                }
                if (cg.alpha < 0)
                {
                    CoinUI.gameObject.SetActive(false);
                    Active = false;
                }
            }
        }
        else
        {
            Player = PhotonFindCurrentClient();
        }
    }
    public void UpdateCoinCount(int CoinCount)
    {
        CoinUI.text = "" + CoinCount;
        cg.alpha = 1f;
        TimerTillFade = TimeToFade;
        Active = true;
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
}
