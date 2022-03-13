using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class votingSystem : MonoBehaviourPun
{
    public static votingSystem init;
    [SerializeField] private showButtonUI emerMeeingUI;
    private checkEndGame isEndGame;
    [SerializeField] private Network network;

    [SerializeField] private GameObject votingWindow;
    [SerializeField] private votingPlayerColumn playerColumnInVote;
    [SerializeField] private Transform votingTab;
    private List<votingPlayerColumn> playerListInVote = new List<votingPlayerColumn>();
    private bool hasVoted;

    private List<int> playerVotedList = new List<int>();
    private List<int> playerGotVotedList = new List<int>();
    private List<int> playerGotKickedList = new List<int>();

    public PhotonView bodyJustFound;
    private List<int> alreadyFoundBodiesList = new List<int>();
    private GameObject[] bodyList;

    [SerializeField] private Button skipButton;

    [SerializeField] private GameObject kickAnnounment;
    [SerializeField] private Text kickedStatus;
    [SerializeField] private Text kickedPlayerName;
    [SerializeField] private Text yourVotedPlayer;

    // timer for vote
    bool timerActive = false;
    float Timer = 10f;


    private void Awake() {
        init = this;
    }

    private void Update() {
        if (timerActive) Timer -= Time.deltaTime;
        if (Timer < 0 && !hasVoted) {
            CastVote(-1);
        }
        if (GameObject.FindGameObjectsWithTag("taskCompleted").Length == 2) {
            network.finishTask();
        }
        
    }


    public bool isFoundBody(int FoundBodyID) {
        return alreadyFoundBodiesList.Contains(FoundBodyID);
    }

    public void triggerEmerMeeting() {   
        photonView.RPC("EmerMeetingAnnouncement", RpcTarget.All);
    }

    [PunRPC]
    public void EmerMeetingAnnouncement() {
        // reset the previous setting from the last meeting
        playerVotedList.Clear();
        playerGotVotedList.Clear();
        yourVotedPlayer.text = string.Empty;
        hasVoted = false;
        
        ToggleAllButtons(true);

        if (!emerMeeingUI.isDead) currentPlayerList();

        if (!emerMeeingUI.isDead) votingWindow.SetActive(true);
        timerActive = true;
    }


    public void triggerReportBody() {
        if (bodyJustFound == null) return;

        // the body has been already found by others
        if (alreadyFoundBodiesList.Contains(bodyJustFound.OwnerActorNr)) return;

        photonView.RPC("ReportBodyAnnouncement", RpcTarget.All, bodyJustFound.OwnerActorNr);
    }

    [PunRPC]
    public void ReportBodyAnnouncement(int FoundBodyID) {
        
        alreadyFoundBodiesList.Add(FoundBodyID);

        GameObject sabo = GameObject.FindGameObjectWithTag("sabotage");
        if (sabo != null) {
            if (sabo.GetComponent<showTaskWindow>().IsActive == true) {
                PhotonView saboState = sabo.GetComponent<PhotonView>();
                saboState.RPC("solvedSaboAnnouncement", RpcTarget.All);
            }
        }

        GameObject critSabo = GameObject.FindGameObjectWithTag("critSabo");
        if (critSabo != null) {
            if (critSabo.GetComponent<showTaskWindow>().IsActive == true) {
                PhotonView critSaboState = critSabo.GetComponent<PhotonView>();
                critSaboState.RPC("solvedSaboAnnouncement", RpcTarget.All);
            }
        }

        

        if (PhotonNetwork.LocalPlayer.ActorNumber == FoundBodyID) {
            bodyList = GameObject.FindGameObjectsWithTag("deadBody");
            foreach(var body in bodyList) {
                if (body.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber) {
                    PhotonNetwork.Destroy(body);
                }   
            }
            return;
        }
        
        // reset the previous setting from the last meeting
        playerVotedList.Clear();
        playerGotVotedList.Clear();
        yourVotedPlayer.text = string.Empty;
        hasVoted = false;
        ToggleAllButtons(true);

        currentPlayerList();

        // cant vote if the player is dead
        if (emerMeeingUI.isDead) return;
        votingWindow.SetActive(true);
        timerActive = true;
    }

    private void ToggleAllButtons(bool canPress) {
        skipButton.interactable = canPress;
        foreach (votingPlayerColumn votePlayer in playerListInVote) {
            votePlayer.toggleButton(canPress);
        }
    }

    public void CastVote(int susPlayerID) {
        // cant vote if player has already voted
        if (hasVoted) return;
        
        timerActive = false;
        Timer = 10f;
        hasVoted = true;
        ToggleAllButtons(false);
        yourVotedPlayer.text = getVotedPlayerName(susPlayerID);
        photonView.RPC("CastMyVoteAnnouncement", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, susPlayerID);
    }

    [PunRPC]
    public void CastMyVoteAnnouncement(int myPlayerID, int ToVoteSusPlayerID) {
        int remainingPlayers = PhotonNetwork.CurrentRoom.PlayerCount    - GameObject.FindGameObjectsWithTag("crewmateDead").Length
                                                                        - GameObject.FindGameObjectsWithTag("impostorDead").Length
                                                                        - GameObject.FindGameObjectsWithTag("crewmateDeadTask").Length;

        foreach (votingPlayerColumn player in playerListInVote) {
            if (player.PlayerID == myPlayerID) {
                // set -1 as skipped
                if (ToVoteSusPlayerID == -1) player.changeStatus("Skipped");
                else player.changeStatus("Voted");
            }
        }

        if (!playerVotedList.Contains(myPlayerID)) {
            playerVotedList.Add(myPlayerID);
            playerGotVotedList.Add(ToVoteSusPlayerID);
        }

        // For the host to count the vote
        if (!PhotonNetwork.IsMasterClient) return;

        // if the vote not yet finish, return
        if (playerVotedList.Count < remainingPlayers) return;

        // Counting the vote
        Dictionary<int, int> playerGotVoteCount = new Dictionary<int, int>();

        foreach (int gotVotedPlayer in playerGotVotedList) {
            if (!playerGotVoteCount.ContainsKey(gotVotedPlayer)) playerGotVoteCount.Add(gotVotedPlayer, 0);
            playerGotVoteCount[gotVotedPlayer]++;
        }

        // find who is voted out
        int votedOutPlayer = -1;
        int mostVoteCount = -1;

        foreach (var playerVote in playerGotVoteCount) {
            if (playerVote.Value > mostVoteCount) {
                mostVoteCount = playerVote.Value;
                votedOutPlayer = playerVote.Key;
            }
        }

        if (remainingPlayers == 5) {
            if (mostVoteCount > 2) {
                photonView.RPC("KickVotedPlayerAnnouncement", RpcTarget.All, votedOutPlayer);
            } else {
                photonView.RPC("KickVotedPlayerAnnouncement", RpcTarget.All, -1);
            }
        }
        else if (remainingPlayers == 4) {
            if (mostVoteCount > 2) {
                photonView.RPC("KickVotedPlayerAnnouncement", RpcTarget.All, votedOutPlayer);
            } else {
                photonView.RPC("KickVotedPlayerAnnouncement", RpcTarget.All, -1);
            }
        }
        else if (remainingPlayers == 3) {
            if (mostVoteCount > 1) {
                photonView.RPC("KickVotedPlayerAnnouncement", RpcTarget.All, votedOutPlayer);
            } else {
                photonView.RPC("KickVotedPlayerAnnouncement", RpcTarget.All, -1);
            }
        }
        else if (remainingPlayers == 2) {
            photonView.RPC("KickVotedPlayerAnnouncement", RpcTarget.All, -1);
        } // debug use
        else if (remainingPlayers == 1) {
            photonView.RPC("KickVotedPlayerAnnouncement", RpcTarget.All, -1);
        }
    }

    [PunRPC]
    public void KickVotedPlayerAnnouncement(int playerID) {
        votingWindow.SetActive(false);
        kickAnnounment.SetActive(true);

        string playerName = "";
        foreach (var p in PhotonNetwork.CurrentRoom.Players) {
            if (p.Value.ActorNumber == playerID) {
                playerName = p.Value.NickName;
                break;
            }
        }

        // if the vote is draw
        if (playerID == -1) {
            kickedPlayerName.text = "No one has been kicked";
        } else {
            kickedPlayerName.text = playerName + " has been kicked";
        }

        

        // finializa the announcement
        if (playerID != -1)playerGotKickedList.Add(playerID);
        StartCoroutine(disableKickAnnouncement(playerID));
    }

    private IEnumerator disableKickAnnouncement(int PlayerID) {
        yield return new WaitForSeconds(2f);

        kickAnnounment.SetActive(false);

        // disconnect the kicked player from the server
        if (PhotonNetwork.LocalPlayer.ActorNumber == PlayerID) {
            kickAnnounment.SetActive(true);
            
            showButtonUI.init.onPlayerBeingKilled();
            network.kickPlayer();
        }
    }

    

    private string getVotedPlayerName(int PlayerID) {
        if (PlayerID == -1) return "Skipped";
        foreach (var player in PhotonNetwork.CurrentRoom.Players) {
            if (player.Value.ActorNumber == PlayerID) return player.Value.NickName;
        }
        return "";
    }

    public void currentPlayerList() {
        for (int i = 0; i < playerListInVote.Count; i++) {
            Destroy(playerListInVote[i].gameObject);
        }
        playerListInVote.Clear();

        foreach (var p in PhotonNetwork.CurrentRoom.Players) {
            // prevent adding player himself to the list
            if (p.Value.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) continue;

            if (playerGotKickedList.Contains(p.Value.ActorNumber)) continue;

            if (alreadyFoundBodiesList.Contains(p.Value.ActorNumber)) continue;

            GameObject[] deadList = GameObject.FindGameObjectsWithTag("crewmateDead");
            GameObject[] deadTaskList = GameObject.FindGameObjectsWithTag("crewmateDeadTask");
            bool check = false;

            foreach (var deadPlayer in deadList) {
                if (deadPlayer.GetComponent<PhotonView>().OwnerActorNr == p.Value.ActorNumber) {
                    check = true;
                    break;
                }
            }

            foreach (var deadPlayer in deadTaskList) {
                if (deadPlayer.GetComponent<PhotonView>().OwnerActorNr == p.Value.ActorNumber) {
                    check = true;
                    break;
                }
            }

            if (check) continue;

            // if the other player is alive, add him to the list
            votingPlayerColumn newPlayerColumn = Instantiate(playerColumnInVote, votingTab);
            newPlayerColumn.initialize(p.Value, this);
            playerListInVote.Add(newPlayerColumn);
        }
    }
}
