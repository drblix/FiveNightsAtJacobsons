using UnityEngine;

// Animatronic that moves in a linear path with no special behaviors

// !! DO NOT EDIT ANYTHING OUTSIDE OF THE INSPECTOR UNLESS YOU KNOW WHAT YOU'RE DOING !!
// !! DO NOT EDIT ANYTHING OUTSIDE OF THE INSPECTOR UNLESS YOU KNOW WHAT YOU'RE DOING !!
// !! DO NOT EDIT ANYTHING OUTSIDE OF THE INSPECTOR UNLESS YOU KNOW WHAT YOU'RE DOING !!
// break something you die by my hand

public class LinearAnimatronic : MonoBehaviour
{
    private GameManager gameManager;
    private CCTVMonitor cctvMonitor;

    [Header("PATH & POSES - MODIFY IF CHANGING PATH")]

    [Tooltip("The animatronic's path; moves from the first element to the last randomly before appearing at the office")]
    [SerializeField]
    private Transform[] movePath;

    [Tooltip("The different poses for the animatronic; MUST MATCH WITH PATH OBJECT'S INDEX!")]
    [SerializeField]
    private GameObject[] poses;

    [SerializeField] 
    [Tooltip("Where the animatronic attempts to attack from")]
    private AccessPoint accessPoint;

    [Tooltip("The activity level of the animatronic; ranges from 0 to 20. (0 is inactive)")]
    public int activity { get; set; }

    [Tooltip("The time it takes for the animatronic to move")]
    public float moveTimer { get; set; }

    [Tooltip("The variation between the moveTimer values (ex: 7 - 1f or 7 + 1f)")]
    private float moveVariation { get; set; }

    [Tooltip("How long the animatronic will wait before attacking")]
    private float attackTimer { get; set; }

    [SerializeField] [Tooltip("How offset the animatronic should be from the player camera when jumpscare is performed")]
    private Vector3 jumpscareOffset;

    private enum AccessPoint
    {
        Hallway,
        LeftVent,
        RightVent
    }

    private int currentPoint = 0;
    private float timer = 0f;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        cctvMonitor = FindObjectOfType<CCTVMonitor>();
        UpdatePoses();
        transform.position = Vector3.zero;

        if (moveVariation > moveTimer)
            moveVariation = moveTimer;
    }

    private void Update()
    {
        enabled = !(activity == 0);

        if (timer > moveTimer + Random.Range(-moveVariation, moveVariation) && currentPoint != movePath.Length - 1)
        {
            // will keep moving until in front of the office
            MoveEvent();

            timer = 0f;
        }
        else if (currentPoint == movePath.Length - 1 && timer > attackTimer && !GameManager.GameOver && GameManager.DoMoveRoll(activity)) 
        {
            // if the animatronic is on their last waypoint, and their way of access
            // is not blocked, kill the player
            gameManager.PlayerDeath(transform, jumpscareOffset);
        }

        timer += Time.deltaTime;
    }

    private void MoveEvent()
    {
        // if can move, move up path by one and change position, etc.
        if (GameManager.DoMoveRoll(activity))
        {
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
                print("tick");
                transform.position = Vector3.zero;
                UpdatePoses();
            }

            // at the end of the path and is now by the office
            if (currentPoint == movePath.Length - 1) 
            {
                Debug.Log("At office entrance");
            }

            StartCoroutine(cctvMonitor.DisconnectCams(Random.Range(1.5f, 4f)));
        }
    }
    
    private void UpdatePoses()
    {
        for (int i = 0; i < poses.Length; i++) {
            if (poses[i]) {
                poses[i].SetActive(false);
            }
        }
    }

    public void SetSettings(AnimatronicSettings settings) {
        activity = settings.activity;
        attackTimer = settings.attackTimer;
        moveTimer = settings.moveTimer;
        moveVariation = settings.moveVariation;
    }
}
