using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Message : MonoBehaviour {

    public TextMeshProUGUI warningMessage;
    public Animator warningMessageAnimator;


    public void ShowWarningMessage(string msg) {
        warningMessage.text = msg;
        warningMessageAnimator.Play("WarningMessage");
    }

}
