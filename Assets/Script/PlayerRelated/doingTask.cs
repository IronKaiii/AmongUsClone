using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class doingTask : MonoBehaviourPun
{
    [SerializeField] private float doingRange = 1f;
    private LineRenderer lineIndicator;
    private showTaskWindow targetTask;

    private void Awake() {
        // only impostor himself can see the killing indicator
        if (!photonView.IsMine) return;

        lineIndicator = GetComponent<LineRenderer>();
        StartCoroutine(searchForTask());
    }

    private void Update() {
        // only impostor himself can see the killing indicator
        if (!photonView.IsMine) return;

        // not showing the indicator when there is no target or target task is finished
        if (targetTask == null || targetTask.IsFinishedTask || !targetTask.IsActive) {
            lineIndicator.SetPosition(0, Vector3.zero);
            lineIndicator.SetPosition(1, Vector3.zero);
        } // else showing the indicator if it can find the target
        else {
            lineIndicator.SetPosition(0, transform.position); // from own position
            lineIndicator.SetPosition(1, targetTask.transform.position); // to target's position
        }
    }

    private IEnumerator searchForTask() {
        // while loop to keep detecting 
        while (true) {
            showTaskWindow checkTarget = null;
            showTaskWindow[] taskList = FindObjectsOfType<showTaskWindow>();

            showButtonUI.init.isTask = false;

            foreach (showTaskWindow temp in taskList) {
                float distance = Vector3.Distance(transform.position, temp.transform.position); 
                
                // check within the doing task distance
                if (distance > doingRange) continue;
                checkTarget = temp;       
                if (!checkTarget.IsActive) {
                    showButtonUI.init.isTask = false;
                }
                else if (!checkTarget.IsFinishedTask) showButtonUI.init.isTask = true;
                else showButtonUI.init.isTask = false;
                
                break;
            }

            // clear the previous targeted task
            if (showButtonUI.init.wantedTask != null && showButtonUI.init.wantedTask != checkTarget) {
                showButtonUI.init.wantedTask.showTask(false);
            }

            targetTask = checkTarget;
            showButtonUI.init.wantedTask = targetTask;

            yield return new WaitForSeconds(0.2f);
        }
    }
}
