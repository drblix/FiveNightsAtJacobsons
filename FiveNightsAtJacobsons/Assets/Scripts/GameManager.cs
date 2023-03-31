using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const int ANIMATRONIC_COUNT = 7;
   
    private const int NIGHT_LENGTH = 540; // seconds
    // private const int HOUR_SUBDIVISIONS = 2; -- for testing
    private const int HOUR_SUBDIVISIONS = NIGHT_LENGTH / 6;

    private Player player;

    [HideInInspector] public UnityEvent<AnimatronicSettings> applySettings;
    [HideInInspector] public UnityEvent gameOverEvent;
    [HideInInspector] public static bool twentyMode = false;
    [HideInInspector] public static bool sixthNight = false;

    public bool gameOver { get; set; } = false;

    private float nightTimer = HOUR_SUBDIVISIONS;
    public int currentHour { get; private set; } = 0; // goes 0 - 5; 0 == 12 AM, 5 == 6 AM

    #region Serializations

    [SerializeField] private TextMeshPro clockText;
    [SerializeField] private TextMeshProUGUI nightDisplay;
    [SerializeField] private AudioSource alarmSound;
    [SerializeField] private GameObject nightFinishedContainer;
    [SerializeField] private GameObject nightFinishedText;
    [SerializeField] private GameObject[] figurines;
    [SerializeField] private bool displaySeconds = false;


    [Header("ANIMATRONIC SETTINGS - USE TO ADJUST DIFFICULTY")]
    
    [SerializeField] private AnimatronicSettings[] night1;
    [SerializeField] private AnimatronicSettings[] night2;
    [SerializeField] private AnimatronicSettings[] night3;
    [SerializeField] private AnimatronicSettings[] night4;
    [SerializeField] private AnimatronicSettings[] night5;
    [SerializeField] private AnimatronicSettings[] night6;
    [SerializeField] private AnimatronicSettings[] twentyNight;
    
    public static AnimatronicSettings[] challengeSettings = null;
    public static int challengeIndex = -1;

    #endregion

    public enum Animatronic
    {
        Wolf,
        Zubek,
        Morrison,
        Roush,
        Flowers,
    }

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        nightDisplay.SetText($"Night {PlayerData.Night}");
        
        if (challengeIndex == 4)
            AudioListener.volume = 0f;
        else
            AudioListener.volume = 1f;

        SetActivities();
        DisplayFigurines();
    }

    private void Update() 
    {
        if (nightTimer <= 0f)
        {
            currentHour++;
            nightTimer = HOUR_SUBDIVISIONS;
            Debug.Log(string.Format("Hour finished : now {0}, was {1}", currentHour, currentHour - 1));

            if (PlayerData.Night > 2)
            {
                foreach (LinearAnimatronic anim in FindObjectsOfType<LinearAnimatronic>())
                {
                    if (anim.activity != 0)
                        anim.activity++;
                }
            }

            if (currentHour >= 6 && !gameOver)
            {
                gameOverEvent.Invoke();
                StartCoroutine(NightFinished());
            }
        }
        
        UpdateClock();
        CheatBinds();

        nightTimer -= Time.deltaTime;
    }

    private void SetActivities()
    {
        if (challengeIndex != -1)
        {
            foreach (AnimatronicSettings setting in challengeSettings)
                applySettings.Invoke(setting);
        }
        else if (twentyMode)
        {
            // special twenty mode settings
            foreach (AnimatronicSettings setting in twentyNight)
                applySettings.Invoke(setting);
        }
        else if (PlayerData.Night != 7)
        {
            AnimatronicSettings[] nightArray;
            switch (PlayerData.Night)
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
                applySettings.Invoke(setting);
        }
        else
        {
            AnimatronicSettings[] settings = new AnimatronicSettings[5];
            float[,] customSettings = DetermineCustomSettings();

            settings[0] = new AnimatronicSettings(Animatronic.Wolf, MainMenu.animActivities[0], customSettings[0,0], customSettings[0,1], customSettings[0,2], customSettings[0,3]);
            settings[1] = new AnimatronicSettings(Animatronic.Morrison, MainMenu.animActivities[1], customSettings[1,0], customSettings[1,1], customSettings[1,2], customSettings[1,3]);
            settings[2] = new AnimatronicSettings(Animatronic.Zubek, MainMenu.animActivities[2], customSettings[2,0], customSettings[2,1], customSettings[2,2], customSettings[2,3]);
            settings[3] = new AnimatronicSettings(Animatronic.Roush, MainMenu.animActivities[3], customSettings[3,0], customSettings[3,1], customSettings[3,2], customSettings[3,3]);
            settings[4] = new AnimatronicSettings(Animatronic.Flowers, MainMenu.animActivities[4], customSettings[4,0], customSettings[4,1], customSettings[4,2], customSettings[4,3]);

            foreach (AnimatronicSettings setting in settings)
                applySettings.Invoke(setting);
        }
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
        
        foreach (Transform child in animatronic)
            child.gameObject.SetActive(false);
        
        GameOverScreen.diedTo = (Animatronic)System.Enum.Parse(typeof(Animatronic), animatronic.name);
        AudioListener.volume = 1f;
        StartCoroutine(player.Jumpscare(animatronic, jumpOffset));
    }

    private IEnumerator NightFinished()
    {
        // Checks if the player beat night 5 or 6
        if (sixthNight)
        {
            if (PlayerData.Stars < 2)
                PlayerData.SetStars(2);
            PlayerData.SetCustom(true);
        }
        else if (twentyMode)
        {
            if (PlayerData.Stars < 3)
                PlayerData.SetStars(3);
        }

        const int CYCLES = 12; // how many times the text flashes
        gameOver = true;

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

        yield return new WaitForSeconds(2f);

        if (challengeIndex != -1)
        {
            bool[] completed = PlayerData.GetPlayerData().completedChallenges;
            completed[challengeIndex] = true;
            PlayerData.SetChallenges(completed);
            SceneManager.LoadScene("Menu");
        }
        else if (sixthNight || twentyMode || PlayerData.Night >= 7)
        {
            sixthNight = false;
            twentyMode = false;
            SceneManager.LoadScene("Menu");
        }
        else if (PlayerData.Night == 5)
        {
            SceneManager.LoadScene("Ending");
        }
        else if (PlayerData.Night <= 4)
        {
            PlayerData.SetNight(PlayerData.Night + 1);
            SceneManager.LoadScene("Main");
        }
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

    private void CheatBinds()
    {
        if (gameOver) { return; }

        if (Keyboard.current.altKey.isPressed && Keyboard.current.nKey.isPressed)
        {   
            if (Keyboard.current.numpad1Key.wasPressedThisFrame || Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                PlayerData.SetNight(1);
                GameManager.sixthNight = false;
                SceneManager.LoadScene("Main");
            }
            else if (Keyboard.current.numpad2Key.wasPressedThisFrame || Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                PlayerData.SetNight(2);
                GameManager.sixthNight = false;
                SceneManager.LoadScene("Main");
            }
            else if (Keyboard.current.numpad3Key.wasPressedThisFrame || Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                PlayerData.SetNight(3);
                GameManager.sixthNight = false;
                SceneManager.LoadScene("Main");
            }
            else if (Keyboard.current.numpad4Key.wasPressedThisFrame || Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                PlayerData.SetNight(4);
                GameManager.sixthNight = false;
                SceneManager.LoadScene("Main");
            }
            else if (Keyboard.current.numpad5Key.wasPressedThisFrame || Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                PlayerData.SetNight(5);
                GameManager.sixthNight = false;
                SceneManager.LoadScene("Main");
            }
            else if (Keyboard.current.numpad6Key.wasPressedThisFrame || Keyboard.current.digit6Key.wasPressedThisFrame)
            {
                PlayerData.SetNight(6);
                GameManager.sixthNight = true;
                SceneManager.LoadScene("Main");
            }
            else if (Keyboard.current.numpad7Key.wasPressedThisFrame || Keyboard.current.digit7Key.wasPressedThisFrame)
            {
                StartCoroutine(NightFinished());
            }
        }
        else if (Keyboard.current.altKey.isPressed && Keyboard.current.pKey.wasPressedThisFrame)
            PowerManager.cheatMode = !PowerManager.cheatMode;
    }

    private float[,] DetermineCustomSettings()
    {
        float[,] settings = new float[5,4];
        for (int x = 0; x < settings.GetLength(0); x++)
        {
            for (int y = 0; y < settings.GetLength(1); y++)
            {
                switch (y)
                {
                    case 0:
                        // move timer
                        settings[x,y] = Mathf.Lerp(14.5f, 4.25f, MainMenu.animActivities[y] / 20f);
                        break;
                    case 1:
                        // move variation
                        settings[x,y] = Mathf.Lerp(3f, 1f, MainMenu.animActivities[y] / 20f);
                        break;
                    case 2:
                        // attack timer
                        settings[x,y] = Mathf.Lerp(10f, 2.75f, MainMenu.animActivities[y] / 20f);
                        break;
                    case 3:
                        // wind-down time
                        settings[x,y] = Mathf.Lerp(210f, 30f, MainMenu.animActivities[y] / 20f);
                        break;
                }
            }
        }

        return settings;
    }

    private void DisplayFigurines()
    {
        SaveData data = PlayerData.GetPlayerData();

        figurines[0].SetActive(data.unlockedCustom);
        figurines[1].SetActive(data.completedChallenges[0]);
        figurines[2].SetActive(data.completedChallenges[1]);
        figurines[3].SetActive(data.completedChallenges[2]);
        figurines[4].SetActive(data.completedChallenges[3]);
        figurines[5].SetActive(data.stars == 3);
        
        bool allCompleted = data.completedChallenges[0] && data.completedChallenges[1] && data.completedChallenges[2] && data.completedChallenges[3] && 
                            data.completedChallenges[4] && data.unlockedChallenges && data.unlockedCustom && data.unlockedSixth && data.stars == 3;
        figurines[6].SetActive(allCompleted);
    }
}

/// <summary>
/// Container for holding various settings for animatronics
/// </summary>
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

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Animatronic Name</param>
    /// <param name="a">Activity</param>
    /// <param name="m_t">Move Timer</param>
    /// <param name="m_v">Move Variation</param>
    /// <param name="a_t">Attack Timer</param>
    /// <param name="w_dt">Wind Down Time</param>
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
