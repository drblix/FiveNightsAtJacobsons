using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialNotification : MonoBehaviour
{
    private GameManager gameManager;

    [TextArea(5, 20)]
    [SerializeField] private string tutorialMsg;

    [SerializeField] private TextMeshProUGUI info;

    private void Awake() {
        if (PlayerData.Night != 1) {
            gameObject.SetActive(false);
        }
        else {
            info.SetText("");
            StartCoroutine(DisplayMessage());
        }
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
