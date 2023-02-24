using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialNotification : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField]
    [TextArea(5, 20)]
    private string tutorialMsg;

    [SerializeField]
    private TextMeshProUGUI info;

    private void Awake() {
        if (GameManager.Night != 1)
            gameObject.SetActive(false);
        
        info.SetText("");
        StartCoroutine(DisplayMessage());
    }

    private void Update() {
        
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            gameObject.SetActive(false);
            enabled = false;
        }
    }

    private IEnumerator DisplayMessage()
    {
        string msg = "";

        for (int i = 0; i < tutorialMsg.Length; i++) {
            msg += tutorialMsg[i];
            info.SetText(msg);
            yield return new WaitForSeconds(.03f);
        }
    }

}