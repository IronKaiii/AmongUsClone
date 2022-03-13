using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class checkEndGame : MonoBehaviourPun
{
    [SerializeField] private GameObject endGameWindow;
    [SerializeField] private Text endGameStatus;
    private int impostorAliveNum;
    private int crewmateAliveNum;
    private int crewmateNotFinishTaskNum;
    private int critSaboNum;



    public void Awake() {
        StartCoroutine(checkEnd());
    }

    private IEnumerator checkEnd() 
    {
        yield return new WaitForSeconds(5f);
        
        while (true) {
            impostorAliveNum =  GameObject.FindGameObjectsWithTag("impostorAlive").Length;
            crewmateAliveNum =  GameObject.FindGameObjectsWithTag("crewmateAlive").Length + 
                                GameObject.FindGameObjectsWithTag("crewmateAliveTask").Length;
            crewmateNotFinishTaskNum =  GameObject.FindGameObjectsWithTag("crewmateAlive").Length +
                                        GameObject.FindGameObjectsWithTag("crewmateDead").Length;
            critSaboNum = GameObject.FindGameObjectsWithTag("critSaboFailed").Length;
            if (impostorAliveNum == 0) {
                crewmateWin();
                break;
            }
            // cancel this if finished debugging
            if (impostorAliveNum >= crewmateAliveNum) {
                impostorWin();
                break;
            }
            // all crewmate finished their task
            if (crewmateNotFinishTaskNum == 0) {
                crewmateWin();
                break;
            }
            if (critSaboNum != 0) {
                impostorWin();
                break;
            }
            yield return new WaitForSeconds(1f);
        }
        
    }

    private void impostorWin() {
        endGameStatus.text = "IMPOSTER";
        endGameWindow.SetActive(true);
    }

    private void crewmateWin(){
        endGameStatus.text = "CREWMATE";
        endGameWindow.SetActive(true);
    }

    
}
