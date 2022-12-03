using UnityEngine;

public class Animatronic : MonoBehaviour
{
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
        if (timer > moveTimer + tVar && currentPoint != movePath.Length - 1)
        {
            MoveEvent();

            timer = 0f;
            tVar = Random.Range(-moveVariation, moveVariation);
        }
        else if (currentPoint == movePath.Length - 1 && timer > attackTimer) 
        {
            Debug.Log("attacking player");
        }

        timer += Time.deltaTime;
    }

    private void MoveEvent()
    {
        int r = Random.Range(0, 20);
        Debug.Log(r);

        if (activity > r)
        {
            currentPoint += 1;
            if (currentPoint <= movePath.Length - 1)
            {
                Waypoint point = movePath[currentPoint];
                transform.position = point.transform.position;
            }

            if (currentPoint == movePath.Length - 1) 
            {
                Debug.Log(animatronicName.ToString() + " is at office entrance");
            }
        }
    }

    public void SetDifficulty(int d) => activity = Mathf.Clamp(d, 0, 20);
}
