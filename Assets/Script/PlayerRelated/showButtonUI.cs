using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showButtonUI : MonoBehaviour
{
    public static showButtonUI init;

    public Button killButton;
    public bool isTarget;
    public canKill player;

    public Button vent2Button;
    public bool canVent;
    public PlayerColor goingVent;


    public Button doTaskButton;
    public bool isTask;
    public showTaskWindow wantedTask;

    public Button reportButton;
    public bool foundBody;

    public Text playerStatusText;
    
    public Button emerMeetingButton;
    public bool canStartEmerMeeting;
    public bool isDead = false;

    public Button startSabotageButton;
    public Button startCritSabotageButton;
    public bool hasUsedSabo = false;
    public bool hasUsedCritSabo = false;
    public bool onGoingSabo = false;


    // For initialization
    private void Awake() {
        init = this;
    }

    private void Update() {
        // check whether player is valid or not
        // show the kill button only to impostor
        if (player != null) {
            if (!isDead) killButton.gameObject.SetActive(player.isImpostor);
            else killButton.gameObject.SetActive(false);
            vent2Button.gameObject.SetActive(player.isImpostor);
            startSabotageButton.gameObject.SetActive(player.isImpostor);
            startCritSabotageButton.gameObject.SetActive(player.isImpostor);
            doTaskButton.gameObject.SetActive(!player.isImpostor);
        }

        // enable the button when there is an in-range target to kill
        killButton.interactable = isTarget;

        // enable the button when there is an in-range task to do
        doTaskButton.interactable = isTask;

        startSabotageButton.interactable = !hasUsedSabo;
        startCritSabotageButton.interactable = !hasUsedCritSabo;

        vent2Button.interactable = canVent;

        // enable the button when there is an in-range dead body
        reportButton.interactable = foundBody;

        // disenable the button when there is critical sabotage
        emerMeetingButton.interactable = (canStartEmerMeeting && !onGoingSabo);
    }

    public void triggerKill() {
        // check whether player is valid or not
        if (player == null) return;

        player.Kill();
    }

    public void triggerVent2() {
        if (goingVent == null) return;
        goingVent.EnterOrExitVent2();
    }

    public void triggerDoTask() {
        // check whether targetedTask is valid or not
        if (wantedTask == null) return;

        wantedTask.showTask(true);
    }

    public void onPlayerBeingKilled() {
        playerStatusText.text = "dead";
        isDead = true;
        
        reportButton.gameObject.SetActive(false);
        canStartEmerMeeting = false;
        canVent = false;
    }

}
