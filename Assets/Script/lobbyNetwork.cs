using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class lobbyNetwork : MonoBehaviourPunCallbacks
{
    private List<roomListUI> allRoomList = new List<roomListUI>();
    [SerializeField] private roomListUI roomColumn;
    [SerializeField] private Transform roomListTab;

    private List<roomListUI> allPlayerList = new List<roomListUI>();
    [SerializeField] private roomListUI playerColumn;
    [SerializeField] private Transform playerListTab;

    [SerializeField] private Text statusText;
    [SerializeField] private Text playerNameText;

    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button startGameButton;

    public static int impostorNum;
    [SerializeField] private Button oneImpostorButton;
    [SerializeField] private Button twoImpostorButton;
    [SerializeField] private Text numOfImpostor;

    


    private void Start() {
        Connect();
        impostorNum = 0; // by default
    }    

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        updateRoomList(roomList);
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "Joined the Lobby";
        Debug.Log("Joined the Lobby");
    }

    public override void OnJoinedRoom()
    {
        updatePlayerList();
        statusText.text = "Joined the Room: " + PhotonNetwork.CurrentRoom.Name;
        Debug.Log("Joined the Room: " + PhotonNetwork.CurrentRoom.Name);
        createRoomButton.interactable = false;

        // only host can start the game
        if (PhotonNetwork.IsMasterClient) {
            startGameButton.interactable = true;
            oneImpostorButton.interactable = true;
            twoImpostorButton.interactable = true;
            // if there are five players in the room, host can choose 1/2 impostor
        }
    }

    // public override void OnLeftRoom()
    // {
    //     createRoomButton.interactable = true;
    //     startGameButton.interactable = false;
    // }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        updatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        updatePlayerList();
    }

    
    private void Connect() {
        PhotonNetwork.NickName = "Player" + Random.Range(0, 1000);
        playerNameText.text = PhotonNetwork.NickName;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void updateRoomList(List<RoomInfo> roomList) {
        for (int i = 0; i < allRoomList.Count; i++) {
            Destroy(allRoomList[i].gameObject);
        }
        allRoomList.Clear();

        for (int i = 0; i < roomList.Count; i++) {
            roomListUI newRoom = Instantiate(roomColumn);
            newRoom.lobbyHost = this;
            newRoom.setName(roomList[i].Name);
            newRoom.transform.SetParent(roomListTab);

            allRoomList.Add(newRoom);
        }
    }

    public void joinRoom(string name) {
        PhotonNetwork.JoinRoom(name);
    }

    public void createRoom() {
        PhotonNetwork.CreateRoom("test" + Random.Range(0, 1000), new RoomOptions() { MaxPlayers = 5 }, null);
        
    }

    private void updatePlayerList() {
        for (int i = 0; i < allPlayerList.Count; i++) {
            Destroy(allPlayerList[i].gameObject);
        }
        allPlayerList.Clear();

        foreach (var p in PhotonNetwork.CurrentRoom.Players) {
            roomListUI newPlayer = Instantiate(playerColumn);

            newPlayer.setName(p.Value.NickName);
            newPlayer.transform.SetParent(playerListTab);

            allPlayerList.Add(newPlayer);
        }
    }

    public void startGame() {
        PhotonNetwork.LoadLevel("inGame");
    }

    public void selectImpostorNum(int num) {
        if (num == 1) {
            impostorNum = num;
            photonView.RPC("impostorNumAnnouncement", RpcTarget.All, "1");
            oneImpostorButton.interactable = false;
            twoImpostorButton.interactable = true;
        }

        if (num == 2) {
            impostorNum = num;
            photonView.RPC("impostorNumAnnouncement", RpcTarget.All, "2");
            oneImpostorButton.interactable = true;
            twoImpostorButton.interactable = false;
        }
    }

    [PunRPC]
    public void impostorNumAnnouncement(string num) {
        numOfImpostor.text = num;
    }
}
