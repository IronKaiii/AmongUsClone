using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Network : MonoBehaviourPunCallbacks
{
    public roomHost roomHost;
    public showButtonUI UIControl;
    public votingSystem votingSystem;
    
    private PhotonView playerPV;

    public int impostorNum;

    public PhotonView PlayerPV {
        get { return playerPV; }
    }

    private void Start()
    {
        // create debug room without entering the lobby first
        if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
        else createInGameRoom();
    }

    private void createInGameRoom() {
        GameObject player = PhotonNetwork.Instantiate("Player",
        new Vector3 (
            Random.Range(1, 4),
            Random.Range(-3, 2),0), Quaternion.identity);

        player.GetComponentInChildren<bodyReport>().initialize(UIControl, votingSystem);
        player.GetComponentInChildren<emerMeetingSystem>().initialize(UIControl, votingSystem);
        playerPV = player.GetComponent<PhotonView>();

        impostorNum = lobbyNetwork.impostorNum;
        if (PhotonNetwork.IsMasterClient) {
            roomHost.Initialize(impostorNum); // DEBUG USE if parameter is int not variable
        }
    }

    // For creating debug room
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.CreateRoom("testing100");
        PhotonNetwork.LocalPlayer.NickName = "Player" + Random.Range(0, 1000);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.JoinRoom("testing100");
        PhotonNetwork.LocalPlayer.NickName = "Player" + Random.Range(0, 1000);
    }

    public override void OnJoinedRoom()
    {
        createInGameRoom();
    }

    public void kickPlayer() {
        playerPV.RPC("deadAnnouncement", RpcTarget.All);
    }

    public void finishTask() {
        playerPV.RPC("TasksFinishedAnnouncement", RpcTarget.All);
    }
}
