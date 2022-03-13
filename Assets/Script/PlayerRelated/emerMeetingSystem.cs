using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class emerMeetingSystem : MonoBehaviourPun
{
    private showButtonUI emerMeetingUI;
    private votingSystem emerVotingSystem;

    public void initialize(showButtonUI UIcontrol, votingSystem votingSystemControl) {
        emerMeetingUI = UIcontrol;
        emerVotingSystem = votingSystemControl;
        emerMeetingUI.canStartEmerMeeting = true;
    }

}
