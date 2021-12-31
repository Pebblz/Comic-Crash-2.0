using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MatchMakingRoomController : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private int PatSceneIndex;

    [SerializeField]
    private GameObject lobbyPanel;

    [SerializeField]
    private GameObject roomPanel;

    [SerializeField]
    private GameObject PatButton;

    [SerializeField]
    private Transform playersContainer;

    [SerializeField]
    private GameObject playerListPrefab;

    [SerializeField]
    private Text roomNameDisplay;

    //all these are to navigate the pause menu with controller. the closed buttons are for 
    //when you close that tag it'll put the xbox navigation over the button to go to the menu
    //just closed 
    [Header("For xbox navigation")]
    public GameObject FirstBTN, FirstBTNBack;
    void ClearPlayerListings()
    {
        for (int i = playersContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(playersContainer.GetChild(i).gameObject);
        }
    }
    void ListPlayers()
    {
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            GameObject tempListing = Instantiate(playerListPrefab, playersContainer);
            Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
            tempText.text = player.NickName;
        }
    }

    public override void OnJoinedRoom()
    {
        roomPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        SetEventSystem(FirstBTN);
        roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
        if(PhotonNetwork.IsMasterClient)
        {
            PatButton.SetActive(true);
        }
        else
        {
            PatButton.SetActive(false);
        }
        ClearPlayerListings();
        ListPlayers();
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        ClearPlayerListings();
        ListPlayers();
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        ClearPlayerListings();
        ListPlayers();
    }
    public void StartGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }
    public void StartPatGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(PatSceneIndex);
        }
    }
    IEnumerator rejoinLobby()
    {
        yield return new WaitForSeconds(1);
        SetEventSystem(FirstBTN);
        PhotonNetwork.JoinLobby();
    }
    public void BackOnClick()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        SetEventSystem(FirstBTNBack);
        StartCoroutine(rejoinLobby());
    }
    void SetEventSystem(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}
