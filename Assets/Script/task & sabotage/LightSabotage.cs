using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightSabotage : MonoBehaviour
{
    private int pressTime = 0;
    public Text display;

    // variable for the signal of the light
    public GameObject greenOn;
    public GameObject redOff;

    // check task resetting
    // player not allow to press during resetting
    private bool isResetting = false;

    private bool isSolved = false;
    public bool IsSolved {
        get {return isSolved; }
    }

    private void OnEnable() {
        isResetting = false;
        pressTime = 0;
        display.text = string.Empty;
        greenOn.SetActive(false);
        redOff.SetActive(false);
    }

    public void ButtonClick(int Press) {
        if (isResetting) return;

        // press once = turn on the light
        pressTime += Press;
        if (pressTime == 1) {
            display.text = "Successful";
            isSolved = true;
            StartCoroutine(ResetTask(isSolved));
        }
    }

    // Resetting the task
    private IEnumerator ResetTask(bool Finished) {
        isResetting = true;
        if (Finished) {
            greenOn.SetActive(true);
            redOff.SetActive(true);
        }

        // Resetting time
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);

        pressTime = 0;
        display.text = string.Empty;
        greenOn.SetActive(false);
        redOff.SetActive(false);
        isResetting = false;
    }

}
