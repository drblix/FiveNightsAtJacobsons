using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialNotification : MonoBehaviour
{    
    private void Awake() {
        if (PlayerData.Night != 1) { // if the current night is not the first night
            gameObject.SetActive(false); // disable the GameObject this script is attached to
        }
    }

    private void Update() {
        if (Keyboard.current.ctrlKey.wasPressedThisFrame) { // if the left mouse button was clicked
            gameObject.SetActive(false); // disable the GameObject this script is attached to
            enabled = false; // disable this script
        }
    }
}
