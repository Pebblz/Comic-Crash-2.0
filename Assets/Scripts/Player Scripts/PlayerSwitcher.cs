using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luminosity.IO;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
public class PlayerSwitcher : MonoBehaviourPun
{
    [Tooltip("This is for the other characters that the player can switch to")]
    public GameObject[] CharactersToSwitchTo = new GameObject[4];

    [SerializeField]
    Transform PlayerTransform;

    GameObject Camera;

    [SerializeField]
    GameObject CurrentPlayer;
    private float dpadX;
    private float dpady;
    float timer;
    [SerializeField] float maxSwitchTimer = .5f;
    public bool CanSwitch;
    public GameObject[] AllCharactersInGame = new GameObject[10];
    Pause pause;
    UnderwaterAirUI ui;
    void Awake()
    {
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        ui = FindObjectOfType<UnderwaterAirUI>();
        pause = GetComponent<Pause>();
        CanSwitch = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (PhotonFindCurrentClient().GetComponent<PhotonView>().IsMine)
        {
            //arrays start at zero so i have to make it one less 
            if (timer < 0)
            {
                if (InputManager.GetButton("1"))
                {
                    SwitchCharacter(0);
                }
                if (InputManager.GetButton("2"))
                {
                    SwitchCharacter(1);
                }
                if (InputManager.GetButton("3"))
                {
                    SwitchCharacter(2);
                }
                if (InputManager.GetButton("4"))
                {
                    SwitchCharacter(3);
                }
            }
        }
        timer -= Time.deltaTime;
    }
    public void ChangeSelectedCharacters(int index, GameObject Character)
    {
        CharactersToSwitchTo[index] = Character;
    }
    public void SwitchToFirstCharacter()
    {
        if(CharactersToSwitchTo[0] != null)
            SwitchCharacter(0);
    }
    void SwitchCharacter(int i)
    {
        if (PhotonFindCurrentClient().GetComponent<PhotonView>().IsMine)
        {
            if (CharactersToSwitchTo[i] != null && CanSwitch)
            {
                CurrentPlayer = PhotonFindCurrentClient();

                PlayerTransform = CurrentPlayer.transform;
                PlayerMovement currentPlayerMovement = CurrentPlayer.GetComponent<PlayerMovement>();

                //currentPlayerMovement.transferOwnership(CurrentPlayer.GetComponent<PhotonView>());

                if (CurrentPlayer.GetComponent<HandMan>())
                {
                    if (CurrentPlayer.GetComponent<HandMan>().isHoldingOBJ)
                    {
                        CurrentPlayer.GetComponent<HandMan>().PickUp.GetComponent<PickUpables>().DropInFront();
                    }
                }

                GameObject Temp = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", CharactersToSwitchTo[i].name),
                    PlayerTransform.position, PlayerTransform.rotation, 0);

                Temp.GetComponent<PhotonView>().ViewID = CurrentPlayer.GetComponent<PhotonView>().ViewID;
                PlayerMovement TempPlayerMovement = Temp.GetComponent<PlayerMovement>();

                Temp.GetComponent<PlayerHealth>().currentAir = CurrentPlayer.GetComponent<PlayerHealth>().currentAir;
                ui.airLeft = CurrentPlayer.GetComponent<PlayerHealth>().currentAir;

                if (!currentPlayerMovement.OnGround && !currentPlayerMovement.Swimming)
                {
                    TempPlayerMovement.PlayFallingAnimation();
                }
                Temp.GetComponent<Player>().respawnPoint =
                    CurrentPlayer.GetComponent<Player>().respawnPoint;

                TempPlayerMovement.jumpPhase = 5;

                Temp.GetComponent<Rigidbody>().velocity = CurrentPlayer.GetComponent<Rigidbody>().velocity;
                if (CurrentPlayer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                {
                    Temp.GetComponent<Animator>().SetBool("Walk", true);
                }
                if (CurrentPlayer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Run"))
                {
                    Temp.GetComponent<Animator>().SetBool("Run", true);
                }
                TempPlayerMovement.OnFloor = currentPlayerMovement.OnFloor;
                if(currentPlayerMovement.InWater)
                {
                    TempPlayerMovement.submergence = currentPlayerMovement.submergence;
                }
                TempPlayerMovement.CanWallJump = false;
                PlayerTransform = Temp.transform;
                Camera.transform.parent = null;
                Camera.GetComponent<MainCamera>().thirdPersonCamera = true;
                Camera.GetComponent<MainCamera>().target = Temp.transform;
                PhotonNetwork.Destroy(CurrentPlayer);
                CurrentPlayer = Temp;
                timer = maxSwitchTimer;
            }
        }
    }
    [PunRPC]
    public void DestroyPlayer()
    {
        PhotonNetwork.Destroy(CurrentPlayer.GetComponent<PhotonView>());
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
