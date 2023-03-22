using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialNotification : MonoBehaviour
{
    [TextArea(5, 20)]
    [SerializeField] private string tutorialMsg; // the tutorial message to be displayed

    [SerializeField] private TextMeshProUGUI info; // reference to a TextMeshProUGUI object to display the message

    private void Awake() {
        if (PlayerData.Night != 1) { // if the current night is not the first night
            gameObject.SetActive(false); // disable the GameObject this script is attached to
        }
        else {
            info.SetText(""); // clear the text of the TextMeshProUGUI object
            StartCoroutine(DisplayMessage()); // start the coroutine to display the tutorial message
        }
    }

    private void Update() {
        
        if (Mouse.current.leftButton.wasPressedThisFrame) { // if the left mouse button was clicked
            gameObject.SetActive(false); // disable the GameObject this script is attached to
            enabled = false; // disable this script
        }
    }

    private IEnumerator DisplayMessage() // coroutine to display the tutorial message
    {
        string msg = ""; // initialize an empty string to gradually build the tutorial message

        for (int i = 0; i < tutorialMsg.Length; i++) { // loop through each character in the tutorial message
            msg += tutorialMsg[i]; // add the current character to the message
            info.SetText(msg); // update the text of the TextMeshProUGUI object to display the current message
            yield return new WaitForSeconds(.03f); // wait for a short amount of time (in seconds)
        }
    }

}
