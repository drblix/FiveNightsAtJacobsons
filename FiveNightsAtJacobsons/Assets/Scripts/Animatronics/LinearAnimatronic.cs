using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Animatronic that moves in a linear path with no special behaviors

// !! DO NOT EDIT ANYTHING OUTSIDE OF THE INSPECTOR UNLESS YOU KNOW WHAT YOU'RE DOING !!
// !! DO NOT EDIT ANYTHING OUTSIDE OF THE INSPECTOR UNLESS YOU KNOW WHAT YOU'RE DOING !!
// !! DO NOT EDIT ANYTHING OUTSIDE OF THE INSPECTOR UNLESS YOU KNOW WHAT YOU'RE DOING !!
// break something you die by my hand

public class LinearAnimatronic : MonoBehaviour
{
    private GameManager gameManager;
    private CCTVMonitor cctvMonitor;
    private SecurityOffice securityOffice;

    [Header("PATH & POSES - MODIFY IF CHANGING PATH")]

    [Tooltip("The animatronic's path; moves from the first element to the last randomly before appearing at the office")]
    [SerializeField] private Transform[] movePath;

    [Tooltip("The different poses for the animatronic; MUST MATCH WITH PATH OBJECT'S INDEX!")]
    [SerializeField] private GameObject[] poses;

    [Tooltip("Where the animatronic attempts to attack from")]
    [SerializeField] private AccessPoint accessPoint;

    [Tooltip("The activity level of the animatronic; ranges from 0 to 20. (0 is inactive)")]
    public int activity { get; set; }

    [Tooltip("The time it takes for the animatronic to move")]
    private float moveTimer { get; set; }

    [Tooltip("The variation between the moveTimer values (ex: 7 - 1f or 7 + 1f)")]
    private float moveVariation { get; set; }

    [Tooltip("How long the animatronic will wait before attacking")]
    private float attackTimer { get; set; }

    [Tooltip("How offset the animatronic should be from the player camera when jumpscare is performed")]
    [SerializeField] private Vector3 jumpscareOffset;

    [Header("Vent Audio")]
    [SerializeField] private AudioClip[] ventClips;
    [SerializeField] private AudioSource ventSource;

    [Header("Zubek specific settings")]
    [SerializeField] private Light officeLight;
    [SerializeField] private Image blackFlash;
    [SerializeField] private AudioSource electricShock;

    private enum AccessPoint
    {
        Hallway,
        LeftVent,
        RightVent
    }

    [SerializeField] private bool bypassNightOne;

    private int currentPoint = 0;
    private float timer = 0f;
    private bool zubekWait = false;
    private bool zubekCanAttack = false;
    [HideInInspector] public bool standingInOffice = false;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        cctvMonitor = FindObjectOfType<CCTVMonitor>();
        securityOffice = FindObjectOfType<SecurityOffice>();

        UpdatePoses();
        transform.position = Vector3.zero;

        if (moveVariation > moveTimer)
            moveVariation = moveTimer;

        gameManager.applySettings.AddListener(SetSettings);
        gameManager.gameOverEvent.AddListener(() => enabled = false);
        FindObjectOfType<PowerManager>().powerOutEvent.AddListener(() => enabled = false);
        securityOffice.shockEvent.AddListener(ShockedReaction);
    }

    private void Update()
    {
        enabled = !(activity == 0);

        if (zubekWait || (PlayerData.Night == 1 && gameManager.currentHour <= 2 && !bypassNightOne)) { return; }
        // if (zubekWait) { return; }

        if (timer > moveTimer + Random.Range(-moveVariation, moveVariation) && currentPoint != movePath.Length - 1)
        {
            // will keep moving until in front of the office
            MoveEvent();

            timer = 0f;
        }
        else if (currentPoint == movePath.Length - 1 && timer > attackTimer && !gameManager.gameOver && GameManager.DoMoveRoll(activity))
        {
            // if the animatronic is on their last waypoint, and their way of access
            // is not blocked, kill the player
            if (accessPoint == AccessPoint.LeftVent)
            {
                if (securityOffice.LeftVentClosed)
                {
                    ventSource.clip = ventClips[Random.Range(0, ventClips.Length)];
                    ventSource.Play();
                    StartCoroutine(VentFade());

                    currentPoint = 0;
                    UpdatePoses();
                    timer = 0f;
                    return;
                }
            }
            else if (accessPoint == AccessPoint.RightVent)
            {
                if (securityOffice.RightVentClosed)
                {
                    ventSource.clip = ventClips[Random.Range(0, ventClips.Length)];
                    ventSource.Play();
                    StartCoroutine(VentFade());

                    currentPoint = 0;
                    UpdatePoses();
                    timer = 0f;
                    return;
                }
            }

            gameManager.PlayerDeath(transform, jumpscareOffset);
        }

        if (accessPoint == AccessPoint.Hallway)
        {
            if (currentPoint != 4)
            {
                timer += Time.deltaTime;
            }
            else if (!cctvMonitor.camerasOpen || zubekCanAttack)
            {
                zubekCanAttack = true;
                if (!standingInOffice)
                {
                    standingInOffice = true;
                    StartCoroutine(FlickerLights());
                }
                timer += Time.deltaTime;
            }
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private void MoveEvent()
    {
        // if can move, move up path by one and change position, etc.
        if (GameManager.DoMoveRoll(activity))
        {
            // Debug.Log(accessPoint == AccessPoint.Hallway && !cctvMonitor.camerasOpen && currentPoint == 3);
            if (accessPoint == AccessPoint.Hallway && !cctvMonitor.camerasOpen && currentPoint == 3)
            {
                zubekWait = true;
                StartCoroutine(ZubekWait());
                return;
            }

            // if not zubek, and currently at a point greater than their second point but less than their 3rd to last point
            if (!transform.name.Equals("Zubek") && (currentPoint > 1 && currentPoint < movePath.Length - 3))
            {
                if (Random.Range(1, 4) == 1)
                    currentPoint--;
                else
                    currentPoint++;
            }
            else
                currentPoint++;

            try
            {
                if (currentPoint <= movePath.Length - 1)
                {
                    Transform point = movePath[currentPoint];
                    transform.position = point.position;
                }

                // updates pose for animatronic
                UpdatePoses();
                poses[currentPoint].SetActive(true);
            }
            catch (UnassignedReferenceException)
            {
                // block is called when at the start of the path if not visible
                transform.position = Vector3.zero;
                UpdatePoses();
            }

            // at the end of the path and is now by the office
            if (currentPoint == movePath.Length - 1)
            {
                Debug.Log("At office entrance");
            }

            if (movePath[currentPoint].name.Contains("Vent") && !ventSource.isPlaying)
            {
                ventSource.clip = ventClips[Random.Range(0, ventClips.Length)];
                ventSource.Play();
            }

            StartCoroutine(cctvMonitor.DisconnectCams(Random.Range(1.5f, 4f)));
        }
    }

    private void UpdatePoses()
    {
        for (int i = 0; i < poses.Length; i++)
        {
            if (poses[i])
            {
                poses[i].SetActive(false);
            }
        }
    }

    public void SetSettings(AnimatronicSettings settings)
    {
        if (settings.animatronicName.ToString().Equals(transform.name))
        {
            activity = settings.activity;
            attackTimer = settings.attackTimer;
            moveTimer = settings.moveTimer;
            moveVariation = settings.moveVariation;

            if (poses[0])
                poses[0].SetActive(true);
            if (movePath[0])
                transform.position = movePath[0].position;

        }
    }

    // Waiting until player opens their cameras then moving into office poised to attack
    private IEnumerator ZubekWait()
    {
        yield return new WaitUntil(() => cctvMonitor.camerasOpen);
        yield return new WaitForSeconds(0.5f);

        currentPoint++;

        UpdatePoses();
        transform.position = movePath[currentPoint].position;
        poses[currentPoint].SetActive(true);

        timer = 0f;
        zubekWait = false;
    }

    private IEnumerator FlickerLights()
    {
        GetComponent<AudioSource>().Play();

        while (standingInOffice)
        {
            officeLight.enabled = false;
            yield return new WaitForSeconds(Random.Range(.05f, .1f));
            officeLight.enabled = true;
            yield return new WaitForSeconds(Random.Range(.01f, .15f));
        }

        GetComponent<AudioSource>().Stop();
        officeLight.enabled = true;
    }

    private void ShockedReaction()
    {
        if (accessPoint == AccessPoint.Hallway && standingInOffice)
        {
            StartCoroutine(BlackFlash());
            electricShock.Play();

            // Send to start with a black flash on player's view
            // Reset standingInOffice = false, timer = 0f, zubekCanAttack = false
            standingInOffice = false;
            timer = 0f;
            zubekCanAttack = false;

            // Resetting poses and position
            currentPoint = 0;
            UpdatePoses();
            transform.position = movePath[currentPoint].position;
            poses[currentPoint].SetActive(true);
        }
    }

    private IEnumerator BlackFlash()
    {
        blackFlash.color = Color.black;
        yield return new WaitForSeconds(.35f);

        float timer = 0f;
        while (timer < 1.25f)
        {
            blackFlash.color = Color.Lerp(Color.black, Color.clear, timer / 1.25f);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator VentFade()
    {
        float clipLength = ventSource.clip.length;
        float ventTimer = 0f;

        while (ventTimer < clipLength)
        {
            ventSource.volume = Mathf.Lerp(1f, 0f, ventTimer / clipLength);

            ventTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        ventSource.volume = 1f;
    }
}
