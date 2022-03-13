using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerColor : Photon.Pun.MonoBehaviourPun, IPunObservable
{
    // making a list of color
    // Five color are added into the list in the Unity
    public List<Color> playerColorsList = new List<Color>();
    public List<Color> playerOutlineColorsList = new List<Color>();
    public SpriteRenderer Body;
    public SpriteRenderer BodyOutline;
    public GameObject PlayerTag;
    public int color;
    public int oriColor;
    public int colorOutline;
    public bool activeTag = true;

    public Text playerName;
 
    [SerializeField] private bool isEnteredVent = false;

    // assign the player color
    private void Awake() {
        if (photonView.IsMine) {
            color = Random.Range(0, playerColorsList.Count-2);
            colorOutline = 0;
            oriColor = color;
            playerName.text = PhotonNetwork.LocalPlayer.NickName + " (me)";
            
        } else {
            playerName.text = otherPlayerName(photonView.OwnerActorNr);
        }
    }
    private void Start() {
        if (!photonView.IsMine) return;
        showButtonUI.init.goingVent = this;
        showButtonUI.init.canVent = true;
    }

    public void EnterOrExitVent2() {
        if (!isEnteredVent) {
            isEnteredVent = true;
            color = playerColorsList.Count-1;
            colorOutline = 1;
            activeTag = false;
        } else {
            isEnteredVent = false;
            color = oriColor;
            colorOutline = 0;
            activeTag = true;
        }
    }

    // player sending his color to other players (Synchronizing)
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        // To 
        if (stream.IsWriting) {
            stream.SendNext(color);
            stream.SendNext(colorOutline);
            stream.SendNext(activeTag);
        }
        else {
            color = (int)stream.ReceiveNext();
            colorOutline = (int)stream.ReceiveNext();
            activeTag = (bool)stream.ReceiveNext();
        }
    }

    // Update the color
    private void Update() {
        Body.color = playerColorsList[color];
        BodyOutline.color = playerOutlineColorsList[colorOutline];
        PlayerTag.SetActive(activeTag);
    }

    private string otherPlayerName(int checkID) {
        foreach (var player in PhotonNetwork.CurrentRoom.Players) {
            if (player.Value.ActorNumber == checkID) return player.Value.NickName;
        }
        return "[error]";
    }

    [PunRPC]
    public void TasksFinishedAnnouncement() {
        
        if (this.gameObject.tag == "crewmateAlive") this.gameObject.tag = "crewmateAliveTask";
        if (this.gameObject.tag == "crewmateDead") this.gameObject.tag = "crewmateDeadTask";
    }
}
