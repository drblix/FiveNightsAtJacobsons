using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Transform mainCam;

    [SerializeField] [Tooltip("Max angle at which the player can rotate side-to-side")]
    private float maxAngle = 60f;

    [SerializeField] [Tooltip("How fast the camera rotates side-to-side")]
    private float camSpeed = 50f;

    private float startingAngle;

    private void Awake() {
        mainCam = Camera.main.transform;
        startingAngle = mainCam.eulerAngles.y;
        // converting the max angle to radians and dividing by 2 to account for
        // quaternion rotations
        maxAngle = Mathf.Deg2Rad * maxAngle / 2f;
    }

    private void Update() {
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());

        Vector3 rotAmount = camSpeed * Time.deltaTime * Vector3.up;

        if (mousePos.x < .2f) {
            mainCam.Rotate(-rotAmount);
        }
        else if (mousePos.x > .8f) {
            mainCam.Rotate(rotAmount);
        }
        
        //mainCam.rotation.y = Mathf.Clamp(mainCam.rotation.y, -maxAngle, maxAngle);
    }
}
