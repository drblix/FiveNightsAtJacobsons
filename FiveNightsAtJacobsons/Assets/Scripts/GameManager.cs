using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const int ANIMATRONIC_COUNT = 5;
   
    private const int NIGHT_LENGTH = 540; // in seconds
    private const int HOUR_SUBDIVISIONS = NIGHT_LENGTH / 6;
    //private const int HOUR_SUBDIVISIONS = 10; -- FOR TESTING --


    private static int[] animatronicActivities = new int[ANIMATRONIC_COUNT];

    private static int currentNight = 1;
    private static bool gameOver = false;
    public static bool GameOver { get { return gameOver; } }
    public static int CurrentNight { get { return currentNight; } }

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

    #endregion

    public bool gameStarted = false;

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
        const int CYCLES = 15; // how many times the text flashes

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
        int seconds = Mathf.FloorToInt(60 - Mathf.Lerp(0, 60, nightTimer / HOUR_SUBDIVISIONS));
        
        clockText.SetText((hours + ":" + seconds.ToString("00")));
    }

    // used for applying activity levels
    public static void ApplyActivities(int[] newA)
    {

    }
}
