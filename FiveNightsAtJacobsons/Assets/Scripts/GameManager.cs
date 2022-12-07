using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int ANIMATRONIC_COUNT = 4;

    private static int[] animatronicActivities = new int[ANIMATRONIC_COUNT];

    private static int currentNight = 1;
    private static bool gameOver = false;
    public static bool GameOver { get { return gameOver; } }
    public static int CurrentNight { get { return currentNight; } }

    private void Awake() 
    {
        
    }

    // method that streamlines move rolling for animatronics
    // returns true if roll succeeded (animatronic can move)
    public static bool DoMoveRoll(int activity)
    {
        int roll = Random.Range(0, 20);
        return activity > roll;
    }

    // static function here for now
    // has no purpose currently
    public static void PlayerDeath()
    {
        gameOver = true;
        Debug.Log("Game over");
    }

    public static void ApplyActivities(int[] newA)
    {

    }
}
