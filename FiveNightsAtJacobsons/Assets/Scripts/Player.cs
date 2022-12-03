using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Transform mainCam;

    [Header("Player Settings")]
    [SerializeField]
    [Tooltip("Max angle at which the player can rotate side-to-side")]
    private float maxAngle = 60f;

    [SerializeField]
    [Tooltip("How fast the camera rotates side-to-side")]
    private float camSpeed = 50f;

    private float startingAngle;

    public bool canLook = true;

    private void Awake()
    {
        // assigning variables
        mainCam = Camera.main.transform;
        startingAngle = mainCam.eulerAngles.y;
    }

    private void Update()
    {
        if (canLook) {
            PlayerRotation();
        }
    }

    private void PlayerRotation() 
    {
        // getting mouse position relative to the game viewport
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        // calculating rotation amount vector
        Vector3 rotAmount = camSpeed * Time.deltaTime * Vector3.up;

        // if mouse is on left side of screen, rotate left
        if (mousePos.x < .2f)
        {
            mainCam.Rotate(-rotAmount);
        }
        // if mouse is on right side of screen, rotate right
        else if (mousePos.x > .8f) 
        {
            mainCam.Rotate(rotAmount);
        }

        // calculates the clamp angle using function
        // min and max are relative to the starting angle
        float newY = ClampAngle(mainCam.eulerAngles.y, startingAngle - maxAngle, startingAngle + maxAngle);
        mainCam.rotation = Quaternion.Euler(0f, newY, 0f);
    }

    private float ClampAngle(float angle, float from, float to)
    {
        // if both clamp parameters are positive, clamp angle between both
        if (Mathf.Sign(from) == 1f && Mathf.Sign(to) == 1f)
        {
            return Mathf.Clamp(angle, from, to);
        }
        // adds 360 if angle < 0f; ensures rotations stay positive in this context
        else if (angle < 0f)
        {
            angle += 360f;
        }
        // returns max value between angle and (from + 360f) value
        else if (angle > 180f)
        {
            return Mathf.Max(angle, 360f + from);
        }

        // otherwise, returns min value between angle and to value
        return Mathf.Min(angle, to);
    }
}
