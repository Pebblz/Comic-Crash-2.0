using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Tooltip("This is here for all the diffrent collectibles in the game")]
    [SerializeField]
    collectible collect;

    [Tooltip("This would be here for if we have like a gold coin that would give 5 coins instead of 1")]
    [Range(1, 5)]
    [SerializeField]
    int numberGivenToPlayer;
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            GameObject gm = GameObject.Find("GameManager");
            if (collect == collectible.Coin)
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
