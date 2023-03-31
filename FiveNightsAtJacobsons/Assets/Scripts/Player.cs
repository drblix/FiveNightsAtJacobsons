using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private GameManager gameManager;
    private SecurityOffice office;
    private PhoneScript phoneScript;
    private CCTVMonitor cctvMonitor;

    private PlayerInput playerInput;
    private Transform mainCam;

    [SerializeField] private AudioClip[] flashSounds;
    [SerializeField] private AudioSource deadLight, flashSource;
    [SerializeField] private Light hallFlashlight;
    [SerializeField] private MeshRenderer hallBlocker;

    [Header("Player Settings")] [Tooltip("Max angle at which the player can rotate side-to-side")]
    [SerializeField] private float maxAngle = 60f;

    [Tooltip("How fast the camera rotates side-to-side")]
    [SerializeField] private float camSpeed = 50f;

    [HideInInspector] public bool canInteract = true;

    [HideInInspector] public bool canUseFlashlight = true;

    [HideInInspector] public bool canUseVentLight = true;

    private float bobTime = 0f;
    private Vector3 startingPos;

    private void Awake()
    {
        // assigning variables
        mainCam = Camera.main.transform;
        startingPos = transform.position;
        phoneScript = FindObjectOfType<PhoneScript>();
        cctvMonitor = FindObjectOfType<CCTVMonitor>();
        gameManager = FindObjectOfType<GameManager>();
        office = GetComponent<SecurityOffice>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (canInteract)
        {
            PlayerRotation();

            // if mouse was pressed, perform raycast and send result to office function
            if (Mouse.current.leftButton.wasPressedThisFrame && !PowerManager.powerEmpty)
            {
                GameObject mouseObj = GetMouseGameObject();

                if (mouseObj) {
                    office.ToggleButton(mouseObj);
                    phoneScript.KeyPressed(mouseObj);
                }
            }

            // if mouse was released, disable all lights
            if (Mouse.current.leftButton.wasReleasedThisFrame)
                office.DisableLights();

            bool lightState = (Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed) && canUseFlashlight && !PowerManager.powerEmpty;

            if (!canUseFlashlight && (Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed))
            {
                if (!deadLight.isPlaying)
                    deadLight.Play();
            }
            else
            {
                deadLight.Stop();
            }
            
            if ((Keyboard.current.leftCtrlKey.wasPressedThisFrame && !Keyboard.current.rightCtrlKey.isPressed) || (Keyboard.current.rightCtrlKey.wasPressedThisFrame && !Keyboard.current.leftCtrlKey.isPressed))
            {
                flashSource.clip = flashSounds[0]; 
                flashSource.Play();
            }
            else if ((Keyboard.current.leftCtrlKey.wasReleasedThisFrame && !Keyboard.current.rightCtrlKey.isPressed) || (Keyboard.current.rightCtrlKey.wasReleasedThisFrame && !Keyboard.current.leftCtrlKey.isPressed))
            {
                flashSource.clip = flashSounds[1]; 
                flashSource.Play();
            }

            hallFlashlight.enabled = lightState;
            hallBlocker.enabled = !lightState;
            HeadSway();
        }
        else
        {
            hallBlocker.enabled = true;
            hallFlashlight.enabled = false;
            office.DisableLights();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        
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
            float speed = Mathf.Lerp(1, 0, mousePos.x / .3f);
            mainCam.Rotate(-rotAmount * speed);
        }
        // if mouse is on right side of screen, rotate right
        else if (mousePos.x > .7f)
        {
            float speed = Mathf.Lerp(1, 0, (1f - mousePos.x) / .3f);
            mainCam.Rotate(rotAmount * speed);
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            mainCam.Rotate(rotAmount * .5f);
        }
        else if (Keyboard.current.leftArrowKey.isPressed)
        {
            mainCam.Rotate(-rotAmount * .5f);
        }

        // calculates the clamp angle using function
        // min and max are relative to the starting angle
        float newY = ClampAngle(mainCam.eulerAngles.y, -maxAngle, maxAngle);
        mainCam.rotation = Quaternion.Euler(0f, newY, 0f);
    }

    // new method; should prevent the occasional snapping
    private float ClampAngle(float current, float min, float max)
    {
        float dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
        float hdtAngle = dtAngle * 0.5f;
        float midAngle = min + hdtAngle;

        float offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
        if (offset > 0)
            current = Mathf.MoveTowardsAngle(current, midAngle, offset);
        
        return current;
    }


    /* previous method
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
    */

    private GameObject GetMouseGameObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        // Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1.5f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Player")))
        {
            if (hit.collider)
                return hit.collider.gameObject;
        }

        return null;
    }

    public IEnumerator Jumpscare(Transform animatronic, Vector3 offset)
    {
        const float SHAKE_SPEED = 4f;
        const float MAX_INCLUSIVE = .085f;

        // Safeguard so the camera doesn't block the jumpscare
        // Scenario only happens if the player opens the camera right as the jumpscare happens
        if (cctvMonitor.camerasOpen && !cctvMonitor.monitorAnimating)
            cctvMonitor.ToggleCams(true);
        else
        {
            cctvMonitor.enabled = false;

            foreach (Transform child in cctvMonitor.transform.Find("CCTVSystem"))
                child.gameObject.SetActive(false);

            cctvMonitor.transform.Find("MonitorSprite").GetComponent<UnityEngine.UI.Image>().enabled = false;
        }
        
        transform.position = startingPos;
        canInteract = false;
        gameManager.gameOver = true;

        // plays jumpscare audio
        GetComponent<AudioSource>().Play();
        transform.Find("JumpscareLight").GetComponent<Light>().enabled = true;

        if (!animatronic.Find("Jumpscare")) {
            Debug.LogError("Couldn't find jumpscare object for: " + animatronic.name);
            yield break;
        }

        animatronic.Find("Jumpscare").gameObject.SetActive(true);

        animatronic.SetParent(transform);
        animatronic.localPosition = offset;
        animatronic.localRotation = Quaternion.Euler(Vector3.zero);

        Vector3 animatronicPos = animatronic.position;
        Vector3 camPos = transform.position;
        Vector3 newPos = camPos + new Vector3(Random.Range(-MAX_INCLUSIVE, MAX_INCLUSIVE), Random.Range(-MAX_INCLUSIVE, MAX_INCLUSIVE), Random.Range(-MAX_INCLUSIVE, MAX_INCLUSIVE));

        StartCoroutine(LoadGameOver(animatronic.Find("Jumpscare").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length - .1f));
        float timer = 0f;
        while (timer < 3f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, SHAKE_SPEED * Time.deltaTime);

            if (transform.position == newPos)
                newPos = camPos + new Vector3(Random.Range(-MAX_INCLUSIVE, MAX_INCLUSIVE), Random.Range(-MAX_INCLUSIVE, MAX_INCLUSIVE), Random.Range(-MAX_INCLUSIVE, MAX_INCLUSIVE));

            timer += Time.deltaTime;
            animatronic.position = animatronicPos;
            yield return new WaitForEndOfFrame();
        }

        transform.position = camPos;
    }

    private IEnumerator LoadGameOver(float time)
    {
        yield return new WaitForSeconds(time);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }

    private void HeadSway()
    {
        const float ZERO = 8 * Mathf.PI;

        bobTime += Time.deltaTime;
        if (bobTime >= ZERO)
            bobTime = 0f;
        
        float offset = (Mathf.Sin(bobTime / 4) / 4) * (2 * Mathf.Cos(bobTime / 2) / 2);
        Vector3 targetPos = startingPos + new Vector3(0f, offset, 0f);
        transform.position = Vector3.Lerp(startingPos, targetPos, .2f);
    }
}
