using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwitcher : MonoBehaviour
{
    [Tooltip("This is for the other characters that the player can switch to")]
    public GameObject[] CharactersToSwitchTo = new GameObject[4];

    [SerializeField]
    Transform PlayerTransform;

    GameObject Camera;

    [SerializeField]
    GameObject CurrentPlayer;
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
        timer -= Time.deltaTime;
    }
    public void One(InputAction.CallbackContext context)
    {
        if (timer < 0)
        {
            SwitchCharacter(0);
        }
    }
    public void Two(InputAction.CallbackContext context)
    {
        if (timer < 0)
        {
            SwitchCharacter(1);
        }
    }
    public void Three(InputAction.CallbackContext context)
    {
        if (timer < 0)
        {
            SwitchCharacter(2);
        }
    }
    public void Four(InputAction.CallbackContext context)
    {
        if (timer < 0)
        {
            SwitchCharacter(3);
        }
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
