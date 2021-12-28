using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject SinglePlayerBTN;
    [SerializeField] GameObject MultiPlayerBTN;
    bool singlePlayer;
    [SerializeField] GameObject MultiPlayerMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject LobbyController;
    [SerializeField] GameObject RoomController;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnConnectedToMaster()
    {
        SinglePlayerBTN.SetActive(true);
        MultiPlayerBTN.SetActive(true);
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
            PhotonNetwork.JoinRoom(s);
        }
    }
    public override void OnJoinedRoom()
    {
        if (singlePlayer)
            PhotonNetwork.LoadLevel("void 2");
    }
    public void Multiplayer()
    {
        if (!singlePlayer)
        {
            LobbyController.SetActive(true);
            RoomController.SetActive(true);
            mainMenu.SetActive(false);
            MultiPlayerMenu.SetActive(true);
        }
    }
}
