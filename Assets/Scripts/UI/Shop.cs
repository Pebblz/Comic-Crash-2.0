using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
public class Shop : MonoBehaviour
{
    [SerializeField] GameObject ShopPage;
    GameObject player;
    float timer;
    bool active;
    GameManager Gm;
    [SerializeField]
    float ShopButtonCooldown = .2f;
    [SerializeField]
    float distanceAwayToTrigger = 4f;
    private void Start()
    {
        Gm = FindObjectOfType<GameManager>();
        ShopPage.SetActive(false);
    }
    void Update()
    {
        if (player == null)
            player = FindObjectOfType<Player>().gameObject;

        if(Vector3.Distance(transform.position, player.transform.position) < distanceAwayToTrigger)
        {
            if(InputManager.GetKeyDown(KeyCode.Q) && timer <= 0)
            {
                active = !active;
                IsShopOpen(active);
                timer = ShopButtonCooldown;
            }
            if(active && InputManager.GetKeyDown(KeyCode.Escape))
            {
                IsShopOpen(false);
                active = false;
            }
        }
        timer -= Time.deltaTime;
    }
    public void IsShopOpen(bool Open)
    {
        ShopPage.SetActive(Open);
        player.GetComponent<PlayerMovement>().enabled = !Open;
        if (Open)
        {
            Gm.unlockCursor();
        }
        else
        {
            Gm.lockCursor();
        }
    }
}
