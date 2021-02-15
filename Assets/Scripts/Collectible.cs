using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField]
    collectible collect;
    [SerializeField]
    int numberGivenToPlayer;
    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            GameObject gm = GameObject.Find("GameManager");
            if(collect == collectible.Coin)
            {
                gm.GetComponent<GameManager>().coinCount += numberGivenToPlayer;
            }
            if (collect == collectible.MainCollectible)
            {
                gm.GetComponent<GameManager>().CollectibleCount += numberGivenToPlayer;
            }
            Destroy(this.gameObject);
        }
    }
    enum collectible
    {
        Coin,
        MainCollectible
    }
}
