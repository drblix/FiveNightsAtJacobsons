using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CCTVCam : MonoBehaviour
{   
    private const float CAM_SPEED = 25f;

    public bool inUse = false;

    [SerializeField]
    [Tooltip("Max angle at which the player can rotate side-to-side")]
    private float maxAngle = 30f;

    private float startingAngle;

    private void Awake()
    {
        startingAngle = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (inUse) 
        {
            UserRotations();
        }
    }

    private void UserRotations() 
    {
        // REUSING PLAYER CONTROL CODE

        // getting mouse position relative to the game viewport
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        // calculating rotation amount vector
        Vector3 rotAmount = CAM_SPEED * Time.deltaTime * Vector3.up;

        // if mouse is on left side of screen, rotate left
        if (mousePos.x < .2f)
        {
            transform.Rotate(-rotAmount, Space.World);
        }
        // if mouse is on right side of screen, rotate right
        else if (mousePos.x > .8f)
        {
            transform.Rotate(rotAmount, Space.World);
        }

        // calculates the clamp angle using function
        // min and max are relative to the starting angle
        float newY = ClampAngle(transform.eulerAngles.y, startingAngle - maxAngle, startingAngle + maxAngle);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, newY, transform.eulerAngles.z);
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
