using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class canKill : MonoBehaviourPun
{
    public bool isImpostor = false; 

    [SerializeField] private float killingRange = 1f;
    private LineRenderer lineIndicator;
    private canKill target = null;
    
    

    // awake function for killing function
    private void Awake() {
        // only impostor himself can see the killing indicator
        if (!photonView.IsMine) return;
        PhotonNetwork.LocalPlayer.CustomProperties["Team"] = 1;
        lineIndicator = GetComponent<LineRenderer>();
        StartCoroutine(searchForTarget());
    }

    private void Start() {
        // only impostor himself can see the killing indicator
        if (!photonView.IsMine) return;

        // attaching the showKillButton function to the player
        showButtonUI.init.player = this;
    }

    // Update function for showing the killing indicator
    private void Update() {
        // only impostor himself can see the killing indicator
        if (!photonView.IsMine) return;

        // not showing the indicator to NOT IMPOSTOR when there is no target
        if (!isImpostor || target == null) {
            lineIndicator.SetPosition(0, Vector3.zero);
            lineIndicator.SetPosition(1, Vector3.zero);
        } // else showing the indicator if it can find the target
        else {
            lineIndicator.SetPosition(0, transform.position); // from own position
            lineIndicator.SetPosition(1, target.transform.position); // to target's position
        }
    }

    private IEnumerator searchForTarget() {
        // while loop to keep detecting whether the target is in range and killable
        while (true) {
            canKill checkTarget = null;
            canKill[] killableList = FindObjectsOfType<canKill>();

            foreach (canKill temp in killableList) {
                
                // avoid check own character
                if (temp == this) continue;

                // avoid check teammate
                if (temp.isImpostor) continue;

                // avoid check deadPlayer
                if (temp.gameObject.tag == "crewmateDead") continue;

                float distance = Vector3.Distance(transform.position, temp.transform.position); 
                
                // check within the killing distance
                if (distance > killingRange) continue;
                checkTarget = temp;
                if (target != null) {
                    showButtonUI.init.isTarget = true;
                } else {
                    showButtonUI.init.isTarget = false;
                }
                break;
            }
            target = checkTarget;

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void Kill() {
        // check whether the target is valid
        if (target == null) return;
        
        PhotonView state = target.GetComponent<PhotonView>();
        
        state.RPC("deadAnnouncement", RpcTarget.All);
        state.RPC("killAnnouncement", RpcTarget.All);
        
    }

    [PunRPC]
    public void deadAnnouncement() {
        if (this.gameObject.tag == "crewmateAlive") this.gameObject.tag = "crewmateDead";
        if (this.gameObject.tag == "impostorAlive") this.gameObject.tag = "impostorDead";
    }
   
    [PunRPC]
    public void killAnnouncement() {
        // ensure the message is sent to the player
        if (!photonView.IsMine) return;

        
        PlayerDeadBody body = PhotonNetwork.Instantiate("PlayerDeadBody", transform.position, Quaternion.identity).GetComponent<PlayerDeadBody>();  
        
        // the deadbody follows the same color
        PlayerColor playerInfo = GetComponent<PlayerColor>();
        body.setColor(playerInfo.playerColorsList[playerInfo.color]);

        transform.position = new Vector3 (Random.Range(1, 4), Random.Range(-3, 2),0);
        showButtonUI.init.onPlayerBeingKilled();

    }

    [PunRPC]
    public void SetImpostor() {
        isImpostor = true;
        this.gameObject.tag = "impostorAlive";
    }
}
