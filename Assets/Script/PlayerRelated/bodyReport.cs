using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class bodyReport : MonoBehaviourPun
{
    private showButtonUI reportUI;
    private votingSystem reportVotingSystem;

    public void initialize(showButtonUI UIcontrol, votingSystem votingSystemControl) {
        reportUI = UIcontrol;
        reportVotingSystem = votingSystemControl;
    }

    // when the player is in range of finding dead body
    private void OnTriggerEnter(Collider other) {
        // debug use
        if (reportVotingSystem == null) return;

        // check whether the dead body is valid
        if (other.gameObject.GetComponent<PlayerDeadBody>() == null) return;

        // if the player is the first person to find the body
        if (!reportVotingSystem.isFoundBody(photonView.OwnerActorNr)) {
            reportUI.foundBody = true;
            reportVotingSystem.bodyJustFound = other.gameObject.GetComponent<PhotonView>();
        }
    }

    // when the player is out of the range of finding dead body
    private void OnTriggerExit(Collider other) {
        // debug use
        if (reportVotingSystem == null) return;

        if (reportVotingSystem.bodyJustFound == other.gameObject.GetComponent<PhotonView>()) {
            reportUI.foundBody = false;
            reportVotingSystem.bodyJustFound = null;
        }
    }

    // when the player stays in the range of finding dead body
    private void OnTriggerStay(Collider other) {
        // debug use
        if (reportVotingSystem == null) return;

        PhotonView deadBodyPV = other.gameObject.GetComponent<PhotonView>();

        if (reportVotingSystem.isFoundBody(photonView.OwnerActorNr)) {
            if (reportVotingSystem.bodyJustFound == deadBodyPV) reportUI.foundBody = false;
        }
    }
}
