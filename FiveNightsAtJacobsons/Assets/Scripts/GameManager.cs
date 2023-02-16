using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const int ANIMATRONIC_COUNT = 7;
   
    private const int NIGHT_LENGTH = 540; // in seconds
    private const int HOUR_SUBDIVISIONS = NIGHT_LENGTH / 6;
    //private const int HOUR_SUBDIVISIONS = 10; -- FOR TESTING --

    private static int[] animatronicActivities = new int[ANIMATRONIC_COUNT];

    private static int night = 1;
    public static int Night { get { return night; } }
    private static bool gameOver = false;
    public static bool GameOver { get { return gameOver; } }

    private float nightTimer = HOUR_SUBDIVISIONS;
    private int currentHour = 0; // goes 0 - 5; 0 == 12 AM, 5 == 6 AM

    #region Serializations

    [SerializeField]
    private TextMeshPro clockText;
    [SerializeField]
    private AudioSource alarmSound;
    [SerializeField]
    private GameObject nightFinishedContainer;
    [SerializeField]
    private GameObject nightFinishedText;

    [SerializeField]
    private bool displaySeconds = false;

    #endregion

    public bool gameStarted = false;

    [Header("ANIMATRONIC SETTINGS - USE TO ADJUST DIFFICULTY")]
    [Header("0 - Wolf; 1 - Zubek; 2 - Morrison; 3 - Roush; 4 - Flowers")]

    [SerializeField]
    private AnimatronicSettings[] night1;
    [SerializeField]
    private AnimatronicSettings[] night2;
    [SerializeField]
    private AnimatronicSettings[] night3;
    [SerializeField]
    private AnimatronicSettings[] night4;
    [SerializeField]
    private AnimatronicSettings[] night5;
    [SerializeField]
    private AnimatronicSettings[] night6;

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
        Debug.Log("CURRENTLY NIGHT " + night);
        SetActivities();
    }

    private void SetActivities()
    {
        AnimatronicSettings[] nightArray;
        switch (night)
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

        foreach (AnimatronicSettings settings in nightArray) {
            GameObject animatronic = GameObject.Find(settings.animatronicName.ToString());
            
            if (animatronic.TryGetComponent<LinearAnimatronic>(out LinearAnimatronic linearAnimatronic)) {
                linearAnimatronic.SetSettings(settings);
            }
            else if (animatronic.TryGetComponent<PuppetBox>(out PuppetBox puppetBox)) {
                puppetBox.SetSettings(settings);
            }
            else if (animatronic.TryGetComponent<Flowers>(out Flowers flowers)) {
                
            }
        }
    }

    private void Update() 
    {
        if (nightTimer <= 0f)
        {
            currentHour++;
            nightTimer = HOUR_SUBDIVISIONS;
            Debug.Log(string.Format("Hour finished : now {0}, was {1}", currentHour, currentHour - 1));

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

    // has no purpose until implemented
    public static void PlayerDeath()
    {
        if (gameOver) { return; }

        gameOver = true;
        Debug.Log("Game over");
    }

    private IEnumerator NightFinished()
    {
        Debug.Log("Night finished");
        const int CYCLES = 12; // how many times the text flashes

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
    public int activity;
    [Range(0f, 50f)]
    public float moveTimer;
    [Range(0f, 50f)]
    public float moveVariation;
    [Range(0f, 50f)]
    public float attackTimer;

    [Header("Flowers-specific settings")]
    [Range(0f, 200f)]
    public float phaseDelay;

    // ROUSH SPECIFIC SETTINGS
    [Header("Roush-specific settings")]
    [Range(0f, 200f)]
    public float windDownTime;

    public AnimatronicSettings(GameManager.Animatronic name, int a, float m_t, float m_v, float a_t, float w_dt, float p_d)
    {
        this.animatronicName = name;
        this.activity = a; 
        this.moveTimer = m_t;
        this.moveVariation = m_v;
        this.attackTimer = a_t;
        this.windDownTime = w_dt;
        this.phaseDelay = p_d;
    }
}
