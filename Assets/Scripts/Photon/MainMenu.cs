using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject SinglePlayerBTN;
    [SerializeField] GameObject MultiPlayerBTN;
    bool singlePlayer;
    bool joined;
    bool once;
    [SerializeField] GameObject MultiPlayerMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject RoomController;
    //all these are to navigate the pause menu with controller. the closed buttons are for 
    //when you close that tag it'll put the xbox navigation over the button to go to the menu
    //just closed 
    [Header("For xbox navigation")]
    public GameObject MultiPlayerFirstBTN, FirstMenuBTN;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnConnectedToMaster()
    {
        SinglePlayerBTN.GetComponent<Button>().interactable = true;
        MultiPlayerBTN.GetComponent<Button>().interactable = true;
    }
    public void SinglePlayer()
    {
        singlePlayer = true;
        PhotonNetwork.JoinLobby();
        SinglePlayerBTN.GetComponent<Button>().interactable = false;
        MultiPlayerBTN.GetComponent<Button>().interactable = false;
    }
    public override void OnJoinedLobby()
    {
        if (singlePlayer)
        {
            RoomOptions roomOps = new RoomOptions() { IsVisible = false, IsOpen = false, MaxPlayers = 1 };
            string s = "SinglePlayer " + Random.Range(0, 10000);
            PhotonNetwork.CreateRoom(s, roomOps);
        }
    }
    public override void OnJoinedRoom()
    {
        if (singlePlayer)
            joined = true;
    }
    public void Multiplayer()
    {
        RoomController.SetActive(true);
        mainMenu.SetActive(false);
        MultiPlayerMenu.SetActive(true);
        SetEventSystem(MultiPlayerFirstBTN);
    }
    public void MultiplayerBackBTN()
    {
        RoomController.SetActive(false);
        mainMenu.SetActive(true);
        MultiPlayerMenu.SetActive(false);
        SetEventSystem(FirstMenuBTN);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void Update()
    {
        if (singlePlayer && joined)
        {
            if (PhotonNetwork.NetworkClientState == ClientState.Joined)
                if (!once)
                {
                    PhotonNetwork.LoadLevel(1);
                    once = true;
                }
        }
    }
    void SetEventSystem(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}
