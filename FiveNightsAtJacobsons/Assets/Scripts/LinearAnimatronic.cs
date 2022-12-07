using UnityEngine;

// Animatronic that moves in a linear path with no special behaviors
// !! DO NOT EDIT ANYTHING OUTSIDE OF THE !!
// !! INSPECTOR UNLESS YOU KNOW WHAT YOU'RE DOING !!
// poses will be handled by the skinned mesh renderer
// with multiple gameobjects that can activated/deactivated
public class LinearAnimatronic : MonoBehaviour
{
    // enum for later
    // will likely be removed in favor of a different system
    public enum AnimatronicNames 
    {
        Demo1,
        Demo2,
        Demo3,
        Demo4,
    }

    [SerializeField]
    private AnimatronicNames animatronicName;

    [Tooltip("The animatronic's path; moves from the first element to the last randomly before appearing at the office")]
    [SerializeField]
    private Waypoint[] movePath;

    [Header("Activity Settings")]

    [Tooltip("The activity level of the animatronic; ranges from 0 to 20. (0 is inactive)")]
    [SerializeField]
    [Range(0, 20)]
    private int activity = 0;

    [Tooltip("The time it takes for the animatronic to move")]
    [SerializeField]
    [Range(3f, 20f)]
    private float moveTimer = 7f;

    [SerializeField]
    [Tooltip("The variation between the moveTimer values (ex: 7 - 1f or 7 + 1f)")]
    [Range(0f, 3f)]
    private float moveVariation = 1.5f;

    [SerializeField]
    [Tooltip("How long the animatronic will wait before attacking")]
    [Range(4f, 15f)]
    private float attackTimer = 10f;

    [Header("Pose Settings")]
    [SerializeField]
    private Vector3[] posePositions;


    private int currentPoint = 0;
    private float timer = 0f;
    private float tVar = 0f;

    private void Awake()
    {
        tVar = Random.Range(-moveVariation, moveVariation);
        transform.position = movePath[currentPoint].transform.position;
    }

    private void Update()
    {
        enabled = !(activity == 0);

        if (timer > moveTimer + tVar && currentPoint != movePath.Length - 1)
        {
            // will continously keep moving until is in front of the office
            MoveEvent();

            timer = 0f;
            tVar = Random.Range(-moveVariation, moveVariation);
        }
        else if (currentPoint == movePath.Length - 1 && timer > attackTimer && !GameManager.GameOver) 
        {
            // if the animatronic is on their last waypoint, and their way of access
            // is not blocked, kill the player
            GameManager.PlayerDeath();
        }

        timer += Time.deltaTime;
    }

    private void MoveEvent()
    {
        // if can move, move up path by one and change position, etc.
        if (GameManager.DoMoveRoll(activity))
        {
            currentPoint += 1;
            if (currentPoint <= movePath.Length - 1)
            {
                Waypoint point = movePath[currentPoint];
                transform.position = point.transform.position;
            }

            // at the end of the path and is now by the office
            if (currentPoint == movePath.Length - 1) 
            {
                Debug.Log(animatronicName.ToString() + " is at office entrance");
            }
        }
    }

    public void SetActivity(int d) => activity = Mathf.Clamp(d, 0, 20);
}
