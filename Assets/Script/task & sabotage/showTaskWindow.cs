using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class showTaskWindow : MonoBehaviourPun
{
    [SerializeField] private GameObject taskWindow;
    [SerializeField] private bool isFinished = false;
    [SerializeField] private bool isActive = false;

    // timer 
    private bool timerActive = false;
    private float Timer = 20f;

    public bool IsFinishedTask {
        get { return isFinished; }
    }

    public bool IsActive {
        get { return isActive; }
    }

    private void Start() {
        if (taskWindow.GetComponent<pressTask>() != null) {
            this.isActive = true;
        }
        if (taskWindow.GetComponent<passwordTask>() != null) {
            this.isActive = true;
        }
        if (taskWindow.GetComponent<LightSabotage>() != null) {
            this.isActive = false;
        }
        if (taskWindow.GetComponent<ResetSabotage>() != null) {
            this.isActive = false;
        }
    }

    public void triggerSabo() {
        if (photonView != null) {
            photonView.RPC("SaboAnnouncement", RpcTarget.All);
        }
    }

    [PunRPC]
    public void SaboAnnouncement() {
        if (taskWindow.GetComponent<LightSabotage>() != null) {
            this.isActive = true;
            showButtonUI.init.hasUsedSabo = true;
            showButtonUI.init.onGoingSabo = true;
        }
    }

    public void triggerCritSabo() {
        timerActive = true;
        if (photonView != null) {
            photonView.RPC("CritSaboAnnouncement", RpcTarget.All);
        }
    }

    [PunRPC]
    public void CritSaboAnnouncement() {
        if (taskWindow.GetComponent<ResetSabotage>() != null) {
            this.isActive = true;
            showButtonUI.init.hasUsedCritSabo = true;
            showButtonUI.init.onGoingSabo = true;
        }
    }

    public void showTask(bool isActive) {
        taskWindow.SetActive(isActive);
    }

    public void Update() {
        if (timerActive) Timer -= Time.deltaTime;

        if (taskWindow.GetComponent<pressTask>() != null) {
            if(taskWindow.GetComponent<pressTask>().IsFinished) {
                isFinished = true;
                this.gameObject.tag = "taskCompleted";
            }
        }
        if (taskWindow.GetComponent<passwordTask>() != null) {
            if (taskWindow.GetComponent<passwordTask>().IsFinished) {
                isFinished = true;
                this.gameObject.tag = "taskCompleted";
            }
        }
        if (taskWindow.GetComponent<LightSabotage>() != null) {
            if (taskWindow.GetComponent<LightSabotage>().IsSolved) {
                photonView.RPC("solvedSaboAnnouncement", RpcTarget.All);
            }
        }
        if (taskWindow.GetComponent<ResetSabotage>() != null) {
            if (!taskWindow.GetComponent<ResetSabotage>().IsSolved && Timer < 0) {
                photonView.RPC("FailedCritSaboAnnouncement", RpcTarget.All);
                timerActive = false;
            }
            if (taskWindow.GetComponent<ResetSabotage>().IsSolved) {
                photonView.RPC("solvedSaboAnnouncement", RpcTarget.All);
            }
        }
        
    }

    [PunRPC]
    public void solvedSaboAnnouncement() {
        this.isFinished = true;
        this.gameObject.tag = "sabotageIsSolved";
        showButtonUI.init.onGoingSabo = false;
    }

    [PunRPC]
    public void FailedCritSaboAnnouncement() {
        if (this.gameObject.tag == "sabotageIsSolved") return;
        this.isFinished = true;
        this.gameObject.tag = "critSaboFailed";
    }
}
