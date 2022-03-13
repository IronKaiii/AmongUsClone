using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pressTask : MonoBehaviour
{
    private int pressTime = 0;
    public Text time;
    public Text display;
    [SerializeField ]private bool isFinished = false;
    
    public bool IsFinished {
        get {return isFinished; }
    }

    // check whether the task is resetting
    private bool isResetting = false;

    private void OnEnable() {
        isResetting = false;
        time.text = string.Empty;
        display.text = string.Empty;
        pressTime = 0;
    }

    public void ButtonClick(int PressNum) {
        if (isResetting) return;

        pressTime += PressNum;
        time.text = pressTime.ToString();

        if (pressTime == 5) {
            display.text = "Successful";
            isFinished = true;
            StartCoroutine(ResetPress());
        }
    }

    // Reset the task
    private IEnumerator ResetPress() {
        isResetting = true;
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        isResetting = false;
    }
}
