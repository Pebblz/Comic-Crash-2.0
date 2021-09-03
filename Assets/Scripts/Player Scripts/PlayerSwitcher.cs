using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
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
    public bool CanSwitch;
    public GameObject[] AllCharactersInGame = new GameObject[10];
    Pause pause;
    void Awake()
    {
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
        pause = GetComponent<Pause>();
        CanSwitch = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        dpadX = Input.GetAxis("Dpad Horizontal");
        dpady = Input.GetAxis("DPad Vertical");

        //arrays start at zero so i have to make it one less 
        if (timer < 0)
        {
            if (Input.GetButtonDown("1") || dpadX == -1)
            {
                SwitchCharacter(0);
            }
            if (Input.GetButtonDown("2") || dpady == 1)
            {
                SwitchCharacter(1);
            }
            if (Input.GetButtonDown("3") || dpadX == 1)
            {
                SwitchCharacter(2);
            }
            if (Input.GetButtonDown("4") || dpady == -1)
            {
                SwitchCharacter(3);
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

        if (i + 1 <= CharactersToSwitchTo.Length && CharactersToSwitchTo[i] != null && CanSwitch)
        {
            if(CurrentPlayer.GetComponent<HandMan>())
            {
               if( CurrentPlayer.GetComponent<HandMan>().isHoldingOBJ)
                {
                    CurrentPlayer.GetComponent<HandMan>().PickUp.GetComponent<PickUpables>().DropInFront();
                }
            }

            GameObject Temp = Instantiate(CharactersToSwitchTo[i],
                PlayerTransform.position, PlayerTransform.rotation);
            if (!CurrentPlayer.GetComponent<PlayerMovement>().OnGround)
            {
                Temp.GetComponent<PlayerMovement>().PlayFallingAnimation();
            }
            Temp.GetComponent<Player>().respawnPoint =
                CurrentPlayer.GetComponent<Player>().respawnPoint;

            Temp.GetComponent<PlayerMovement>().jumpPhase = 5;

            Temp.GetComponent<Rigidbody>().velocity = CurrentPlayer.GetComponent<Rigidbody>().velocity;

            if (CurrentPlayer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                Temp.GetComponent<Animator>().SetBool("Walk", true);
            }
            if (CurrentPlayer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                Temp.GetComponent<Animator>().SetBool("Run", true);
            }
            Temp.GetComponent<PlayerMovement>().CanWallJump = false;
            PlayerTransform = Temp.transform;
            Camera.transform.parent = null;
            Camera.GetComponent<MainCamera>().thirdPersonCamera = true;
            Camera.GetComponent<MainCamera>().target = Temp.transform;
            Destroy(CurrentPlayer);
            CurrentPlayer = Temp;
            timer = .2f;
        }
    }
}
