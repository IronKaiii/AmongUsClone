using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class votingPlayerColumn : MonoBehaviour
{
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text playerStatusText;

    private Button voteButton;
    private votingSystem VotingSystem;
    private int playerID;

    public int PlayerID {
        get { return playerID; }
    }

    private void Awake() {
        voteButton = GetComponentInChildren<Button>();
        voteButton.onClick.AddListener(triggerVote);
    }

    private void triggerVote() {
        VotingSystem.CastVote(playerID);
    }

    public void initialize(Player player, votingSystem Voting) {
        playerID = player.ActorNumber;
        playerNameText.text = player.NickName;
        playerStatusText.text = "Not vote";
        VotingSystem = Voting;
    }

    public void changeStatus(string newStatus) {
        playerStatusText.text = newStatus;
    }

    public void toggleButton(bool canPress) {
        voteButton.interactable = canPress;
    }
}
