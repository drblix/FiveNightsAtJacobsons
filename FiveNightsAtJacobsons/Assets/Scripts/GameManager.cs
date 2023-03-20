using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const int ANIMATRONIC_COUNT = 7;
   
    private const int NIGHT_LENGTH = 540; // seconds
    private const int HOUR_SUBDIVISIONS = NIGHT_LENGTH / 6;

    private Player player;

    [HideInInspector] public UnityEvent<AnimatronicSettings> applySettings;
    [HideInInspector] public UnityEvent gameOverEvent;
    [HideInInspector] public static bool twentyMode = false;
    [HideInInspector] public static bool sixthNight = false;

    public bool gameOver { get; set; } = false;

    private float nightTimer = HOUR_SUBDIVISIONS;
    private int currentHour = 0; // goes 0 - 5; 0 == 12 AM, 5 == 6 AM

    #region Serializations

    [SerializeField] private TextMeshPro clockText;
    [SerializeField] private AudioSource alarmSound;
    [SerializeField] private GameObject nightFinishedContainer;
    [SerializeField] private GameObject nightFinishedText;
    [SerializeField] private bool displaySeconds = false;


    [Header("ANIMATRONIC SETTINGS - USE TO ADJUST DIFFICULTY")]
    
    [SerializeField] private AnimatronicSettings[] night1;
    [SerializeField] private AnimatronicSettings[] night2;
    [SerializeField] private AnimatronicSettings[] night3;
    [SerializeField] private AnimatronicSettings[] night4;
    [SerializeField] private AnimatronicSettings[] night5;
    [SerializeField] private AnimatronicSettings[] night6;

    #endregion

    public enum Animatronic
    {
        Wolf,
        Zubek,
        Morrison,
        Roush,
        Flowers,
        Gammon
    }

    private void Awake()
    {
        player = FindObjectOfType<Player>();

        Debug.Log("CURRENTLY NIGHT " + PlayerData.night);
        SetActivities();
    }

    private void SetActivities()
    {
        AnimatronicSettings[] nightArray;
        switch (PlayerData.night)
        {
            case 1:
                nightArray = night1;
                break;
            case 2:
                nightArray = night2;
                break;
            case 3:
                nightArray = night3;
                break;
            case 4:
                nightArray = night4;
                break;
            case 5:
                nightArray = night5;
                break;
            case 6:
                nightArray = night6;
                break;
            default:
                nightArray = night1;
                break;
        }

        foreach (AnimatronicSettings setting in nightArray)
        {
            applySettings.Invoke(setting);
        }
    }

    private void Update() 
    {
        if (nightTimer <= 0f)
        {
            currentHour++;
            nightTimer = HOUR_SUBDIVISIONS;
            Debug.Log(string.Format("Hour finished : now {0}, was {1}", currentHour, currentHour - 1));

            foreach (LinearAnimatronic anim in FindObjectsOfType<LinearAnimatronic>())
            {
                if (anim.activity != 0)
                    anim.activity++;
            }

            if (currentHour == 5)
            {
                StartCoroutine(NightFinished());
            }
        }
        
        UpdateClock();
        nightTimer -= Time.deltaTime;
    }

    // method that streamlines move rolling for animatronics
    // returns true if roll succeeded (animatronic can move)
    public static bool DoMoveRoll(int activity)
    {
        int roll = Random.Range(0, 21);
        return activity > roll;
    }

    public void PlayerDeath(Transform animatronic, Vector3 jumpOffset)
    {
        if (gameOver) { return; }

        gameOverEvent.Invoke();
        gameOver = true;
        
        Debug.Log("Game over");
        foreach (Transform child in animatronic)
            child.gameObject.SetActive(false);
        
        StartCoroutine(player.Jumpscare(animatronic, jumpOffset));
    }

    private IEnumerator NightFinished()
    {
        Debug.Log("Night finished!");
        // Checks if the player beat night 5 or 6
        if (PlayerData.night == 5)
        {
            PlayerData.SetStars(1);
        }
        else if (sixthNight)
        {
            PlayerData.SetStars(2);
            sixthNight = false;
        }
        else if (twentyMode)
        {
            PlayerData.SetStars(3);
            twentyMode = false;
        }

        PlayerData.SetNight(PlayerData.night + 1);

        const int CYCLES = 12; // how many times the text flashes
        gameOver = true;
        gameOverEvent.Invoke();

        // mutes all other sounds in the game
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            if (source != alarmSound)
                source.volume = 0f;
        }

        nightFinishedContainer.SetActive(true);

        for (int i = 0; i < CYCLES * 2; i++)
        {
            nightFinishedText.SetActive(!nightFinishedText.activeInHierarchy);
            if (nightFinishedText.activeInHierarchy)
                alarmSound.Play();
            yield return new WaitForSeconds(.25f);
        }

        alarmSound.Stop();        
    }

    private void UpdateClock()
    {
        // if currentHour is 0, assign 12 instead
        int hours = currentHour != 0 ? currentHour : 12;
        // lerping the seconds for each hour duration
        int seconds = 0;
        if (displaySeconds)
            seconds = Mathf.FloorToInt(60 - Mathf.Lerp(0, 60, nightTimer / HOUR_SUBDIVISIONS));
        
        clockText.SetText((hours + ":" + seconds.ToString("00")));
    }
}

[System.Serializable]
public struct AnimatronicSettings
{
    public GameManager.Animatronic animatronicName;
    [Range(0, 20)]
    [Tooltip("Animatronic's activity level; used in move rolls to determine if the animatronic can move")]
    public int activity;
    [Range(0f, 50f)]
    [Tooltip("Time it takes for the animatronic to move down their path")]
    public float moveTimer;
    [Range(0f, 50f)]
    [Tooltip("The variation amount between each move; basically the randomization threshold for the move timer; 0 for a constant move time")]
    public float moveVariation;
    [Range(0f, 50f)]
    [Tooltip("How long it takes for an animatronic to attack while at the office")]
    public float attackTimer;

    [Header("Roush-only settings")]
    [Range(0, 500)]
    [Tooltip("The length in seconds it takes to wind down to 0")]
    public float windDownTime;

    public AnimatronicSettings(GameManager.Animatronic name, int a, float m_t, float m_v, float a_t, float w_dt)
    {
        this.animatronicName = name;
        this.activity = a; 
        this.moveTimer = m_t;
        this.moveVariation = m_v;
        this.attackTimer = a_t;
        this.windDownTime = w_dt;
    }
}
