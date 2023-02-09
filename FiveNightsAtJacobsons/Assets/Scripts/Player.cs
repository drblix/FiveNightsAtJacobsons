using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private SecurityOffice office;
    private PhoneScript phoneScript;

    private Transform mainCam;

    [Header("Player Settings")]
    [SerializeField]
    [Tooltip("Max angle at which the player can rotate side-to-side")]
    private float maxAngle = 60f;

    [SerializeField]
    [Tooltip("How fast the camera rotates side-to-side")]
    private float camSpeed = 50f;

    private float startingAngle = 0f;

    [HideInInspector]
    public bool canLook = true;

    private void Awake()
    {
        // assigning variables
        mainCam = Camera.main.transform;
        office = GetComponent<SecurityOffice>();
        phoneScript = FindObjectOfType<PhoneScript>();
    }

    private void Update()
    {
        if (canLook)
        {
            PlayerRotation();

            // if mouse was pressed, perform raycast and send result to office function
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                GameObject mouseObj = GetMouseGameObject();

                if (mouseObj) {
                    office.ToggleButton(mouseObj);
                    phoneScript.KeyPressed(mouseObj);
                }
            }

            // if mouse was released, disable all lights by default
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                office.DisableLights();
            }
        }
    }

    private void PlayerRotation()
    {
        // getting mouse position relative to the game viewport
        Vector2 mousePos = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        // calculating rotation amount vector
        Vector3 rotAmount = camSpeed * Time.deltaTime * Vector3.up;

        // if mouse is on left side of screen, rotate left
        if (mousePos.x < .3f)
        {
            mainCam.Rotate(-rotAmount);
        }
        // if mouse is on right side of screen, rotate right
        else if (mousePos.x > .7f)
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

    private GameObject GetMouseGameObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1.5f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Player")))
        {
            if (hit.collider)
                return hit.collider.gameObject;
        }

        return null;
    }
}
