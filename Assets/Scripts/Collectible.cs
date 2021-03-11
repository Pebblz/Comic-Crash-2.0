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
                Destroy(this.gameObject);
            }
            else if (collect == collectible.MainCollectible)
            {
                gm.GetComponent<GameManager>().CollectibleCount += numberGivenToPlayer;
                Destroy(this.gameObject);
            }
            else if(collect == collectible.HeartOne)
            {
                if (col.gameObject.GetComponent<PlayerHealth>().currentHealth != col.gameObject.GetComponent<PlayerHealth>().maxHealth)
                {
                    col.gameObject.GetComponent<PlayerHealth>().currentHealth += 1;
                    Destroy(this.gameObject);
                } else
                {
                    print("Your at max health dumb dumb");
                }
            } else if(collect == collectible.maxHealthUp)
            {
                col.gameObject.GetComponent<PlayerHealth>().maxHealth += 1;
                col.gameObject.GetComponent<PlayerHealth>().ResetHealth();
                print("new health = " + col.gameObject.GetComponent<PlayerHealth>().maxHealth);
                Destroy(this.gameObject);
            } else if( collect == collectible.FullHeal)
            {
                if (col.gameObject.GetComponent<PlayerHealth>().currentHealth != col.gameObject.GetComponent<PlayerHealth>().maxHealth)
                {
                    col.gameObject.GetComponent<PlayerHealth>().ResetHealth();
                    Destroy(this.gameObject);
                }
                else
                {
                    print("Your at max health dumb dumb");
                }
            }

        }
    }
    enum collectible
    {
        Coin,
        MainCollectible,
        HeartOne,
        FullHeal,
        maxHealthUp
    }
}
