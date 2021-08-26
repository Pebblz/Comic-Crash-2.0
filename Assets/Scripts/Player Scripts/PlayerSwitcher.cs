using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    [Tooltip("This is for the other characters that the player can switch to")]
    public GameObject[] CharactersToSwitchTo = new GameObject[10];

    [SerializeField]
    Transform PlayerTransform;

    GameObject Camera;

    [SerializeField]
    GameObject CurrentPlayer;
    private float dpadX;
    private float dpady;
    float timer;
    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };
    // Start is called before the first frame update
    void Awake()
    {
        Camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        dpadX = Input.GetAxis("Dpad Horizontal");
        dpady = Input.GetAxis("DPad Vertical");
        //for (int i = 0; i < keyCodes.Length; i++)
        //{
        //    if (Input.GetKeyDown(keyCodes[i]) && timer <= 0)
        //    {
        //        SwitchCharacter(i);
        //        timer = .5f;
        //    }
        //}
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
    void SwitchCharacter(int i)
    {

        if (i + 1 <= CharactersToSwitchTo.Length)
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
