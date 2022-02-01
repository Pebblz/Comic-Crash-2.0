using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Luminosity.IO;
public class Shop : MonoBehaviour
{
    GameObject ShopPage;
    GameObject player;
    float timer;
    public bool active;
    GameManager Gm;
    [SerializeField]
    float ShopButtonCooldown = .2f;
    [SerializeField]
    float distanceAwayToTrigger = 4f;
    private void Start()
    {
        Gm = FindObjectOfType<GameManager>();
        ShopPage = GameObject.Find("ShopPage");
        ShopPage.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { CloseShop(); });
        ShopPage.SetActive(false);
    }
    void Update()
    {
        if (player == null)
            player = FindObjectOfType<Player>().gameObject;

        if(Vector3.Distance(transform.position, player.transform.position) < distanceAwayToTrigger)
        {
            if(InputManager.GetButton("Left Mouse") && timer <= 0 && !active)
            {
                active = !active;
                IsShopOpen(active);
                timer = ShopButtonCooldown;
            }
            if(active && InputManager.GetButton("Pause"))
            {
                IsShopOpen(false);
                active = false;
            }
        }
        timer -= Time.deltaTime;
    }
    public void CloseShop()
    {
        active = !active;
        IsShopOpen(active);
        timer = ShopButtonCooldown;
    }
    public void IsShopOpen(bool Open)
    {
        Gm.GetComponent<PlayerSwitcher>().CanSwitch = !Gm.GetComponent<PlayerSwitcher>().CanSwitch;
        ShopPage.SetActive(Open);
        player.GetComponent<PlayerMovement>().CantMove  = !player.GetComponent<PlayerMovement>().CantMove;
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
