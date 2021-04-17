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
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]) && timer <= 0)
            {
                SwitchCharacter(i);
                timer = .5f;
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

            if (!Temp.GetComponent<PlayerMovement>().IsGrounded())
            {
                Temp.GetComponent<PlayerMovement>().PlayFallingAnimation();
            }

            Temp.GetComponent<Player>().respawnPoint =
                CurrentPlayer.GetComponent<Player>().respawnPoint;

            Temp.GetComponent<PlayerMovement>().jumpsMade = 50;

            PlayerTransform = Temp.transform;
            Camera.transform.parent = null;
            Camera.GetComponent<Camera>().thirdPersonCamera = true;
            Camera.GetComponent<Camera>().target = Temp.transform;
            Destroy(CurrentPlayer);
            CurrentPlayer = Temp;

        }
    }
}
