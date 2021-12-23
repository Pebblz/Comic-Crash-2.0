using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject SinglePlayerBTN;
    [SerializeField] GameObject MultiPlayerBTN;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public void SinglePlayer()
    {
        RoomOptions roomOps = new RoomOptions() { IsVisible = false, IsOpen = false, MaxPlayers = 1 };
        string s = "SinglePlayer " + Random.Range(0, 10000);
        PhotonNetwork.CreateRoom(s, roomOps);
        PhotonNetwork.JoinRoom(s);
        //PhotonNetwork.OfflineMode = true;
    }
    public override void OnJoinedLobby()
    {
        SinglePlayerBTN.SetActive(true);
        MultiPlayerBTN.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("void 2");
    }
    public void Multiplayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(3);
        }
    }
}
