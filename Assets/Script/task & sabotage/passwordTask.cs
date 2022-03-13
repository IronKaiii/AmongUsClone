using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class passwordTask : MonoBehaviour
{
    public Text input;
    private bool isFinished = false;
    public bool IsFinished {
        get {return isFinished; }
    }

    private void OnEnable() {
        input.text = string.Empty;
    }

    public void ButtonClick(int num) {
        input.text += num;

        // check if the input is correct or not
        if (input.text == "12345") {
            input.text = "Correct";
            isFinished = true;
            StartCoroutine(ResetDisplay(isFinished));
        }
        else if (input.text.Length >= 5) {
            input.text = "Incorrect";
            StartCoroutine(ResetDisplay(false));
        }
    }

    // timer for resetting
    private IEnumerator ResetDisplay(bool isSuccess) {
        yield return new WaitForSeconds(1f);
        if (isSuccess) gameObject.SetActive(false);
        input.text = string.Empty;
    }
}
