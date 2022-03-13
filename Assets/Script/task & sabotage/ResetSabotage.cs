using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetSabotage : MonoBehaviour
{
    private int pressTime = 0;
    public Text display;

    // check task resetting
    // player not allow to press during resetting
    private bool isResetting = false;
    private bool isSolved = false;
    public bool IsSolved {
        get {return isSolved; }
    }

    private void OnEnable() {
        isResetting = false;
        display.text = "Reset is required.";
        pressTime = 0;
    }

    public void ButtonClick(int Press) {
        if (isResetting) return;

        // press once = turn on the light
        pressTime += Press;
        if (pressTime == 1) {
            display.text = "Successful";
            isSolved = true;
            StartCoroutine(ResetTask());
        }
    }

    private IEnumerator ResetTask() {
        isResetting = true;
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        isResetting = false;
    }

}

