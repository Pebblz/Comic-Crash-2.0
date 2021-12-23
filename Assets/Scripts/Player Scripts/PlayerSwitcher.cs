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
    public bool CanSwitch;
    Pause pause;
    UnderwaterAirUI ui;
    int currentCharacter;

    SoundManager sound;

    void Awake()
    {
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        ui = FindObjectOfType<UnderwaterAirUI>();
        pause = GetComponent<Pause>();
        CanSwitch = true;
        sound = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonFindCurrentClient().GetComponent<PhotonView>().IsMine)
        {
            //arrays start at zero so i have to make it one less 
            if (!pause.isPaused)
            {
                if (InputManager.GetButtonDown("1"))
                {

                    if (currentCharacter > 0)
                    {
                        currentCharacter--;
                    }
                    else
                    {
                        currentCharacter = 2;
                    }
                    SwitchCharacter(currentCharacter);

                }
                if (InputManager.GetButtonDown("2"))
                {

                    if (currentCharacter < 2)
                    {
                        currentCharacter++;
                    }
                    else
                    {
                        currentCharacter = 0;
                    }
                    SwitchCharacter(currentCharacter);

                }
            }
        }
    }
    public void ChangeSelectedCharacters(int index, GameObject Character)
    {
        CharactersToSwitchTo[index] = Character;
    }
    public void SwitchToFirstCharacter()
    {
        if (CharactersToSwitchTo[0] != null)
            SwitchCharacter(0);
    }
    void SwitchCharacter(int i)
    {
        if (PhotonFindCurrentClient().GetComponent<PhotonView>().IsMine)
        {
            if (CharactersToSwitchTo[i] != null && CanSwitch)
            {
                CurrentPlayer = PhotonFindCurrentClient();
                if (!CurrentPlayer.GetComponent<PlayerMovement>().anim.GetCurrentAnimatorStateInfo(0).IsName("Got Collectible"))
                {
                    PlayerTransform = CurrentPlayer.transform;
                    PlayerMovement currentPlayerMovement = CurrentPlayer.GetComponent<PlayerMovement>();


                    if (CurrentPlayer.GetComponent<HandMan>())
                    {
                        if (CurrentPlayer.GetComponent<HandMan>().isHoldingOBJ)
                        {
                            CurrentPlayer.GetComponent<HandMan>().PickUp.GetComponent<PickUpables>().DropInFront();
                        }
                    }

                    GameObject Temp = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", CharactersToSwitchTo[i].name),
                        PlayerTransform.position, PlayerTransform.rotation, 0);

                    Temp.GetComponent<PlayerHealth>().currentHealth = CurrentPlayer.GetComponent<PlayerHealth>().currentHealth;

                    Temp.GetComponent<PhotonView>().ViewID = CurrentPlayer.GetComponent<PhotonView>().ViewID;
                    PlayerMovement TempPlayerMovement = Temp.GetComponent<PlayerMovement>();

                    Temp.GetComponent<PlayerHealth>().currentAir = CurrentPlayer.GetComponent<PlayerHealth>().currentAir;
                    ui.airLeft = CurrentPlayer.GetComponent<PlayerHealth>().currentAir;

                    Temp.GetComponent<Player>().respawnPoint =
                        CurrentPlayer.GetComponent<Player>().respawnPoint;
                    Temp.GetComponent<PlayerMovement>().originalGravity = CurrentPlayer.GetComponent<PlayerMovement>().originalGravity;

                    TempPlayerMovement.jumpPhase = 5;

                    Temp.GetComponent<Rigidbody>().velocity = CurrentPlayer.GetComponent<Rigidbody>().velocity;
                    if (currentPlayerMovement.anim.GetCurrentAnimatorStateInfo(0).IsName("Dive") && currentPlayerMovement.OnGround)
                    {
                        TempPlayerMovement.PlayAnimation("idle");
                    }
                    if (currentPlayerMovement.anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                    {
                        TempPlayerMovement.PlayAnimation("Run");
                    }
                    else
                    {
                        if (currentPlayerMovement.anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                        {
                            TempPlayerMovement.PlayAnimation("Walk");
                        }
                        if (!currentPlayerMovement.OnGround && !currentPlayerMovement.InWater)
                        {
                            TempPlayerMovement.PlayFallingAnimation();
                        }
                    }
                    TempPlayerMovement.OnFloor = currentPlayerMovement.OnFloor;
                    if (currentPlayerMovement.InWater)
                    {
                        TempPlayerMovement.submergence = currentPlayerMovement.submergence;
                        if(Temp.GetComponent<PlayerAttack>())
                            Temp.GetComponent<PlayerAttack>().TimeTillnextAttack = .3f;
                    }
                    TempPlayerMovement.CanWallJump = false;
                    PlayerTransform = Temp.transform;
                    Camera.transform.parent = null;
                    Camera.GetComponent<MainCamera>().thirdPersonCamera = true;
                    Camera.GetComponent<MainCamera>().target = Temp.transform;
                    PhotonNetwork.Destroy(CurrentPlayer);
                    CurrentPlayer = Temp;
                    sound.attach_sounds_to_player(CurrentPlayer);
                }
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
