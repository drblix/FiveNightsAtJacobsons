using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChallengeScreen : MonoBehaviour
{
    // upstairs teachers - flowers, roush, zubek
    // sound is your friend - wolf, morrison, flowers
    // eyes open - flowers, roush, zubek
    // the middle - all on 10
    // something's missing - deaf mode, all on 13
    private static readonly string[] challengeNames = { "Upstairs Teachers", "Sound is your Friend", "Besties", "The Middle", "Something's Missing..." };
    private int currentChallenge = 0;

    [SerializeField] private Color completedChallenge;
    [SerializeField] private Color incompletedChallenge;

    [SerializeField] private TextMeshProUGUI challengeDisplay, startBtn;
    [SerializeField] private Image[] teacherPortraits;

    [SerializeField] private AnimatronicSettings[] upstairsTeachers, soundIsYourFriend, besties, theMiddle, somethingsMissing;
    private AnimatronicSettings[] chosenSettings;

    private void Start()
    {
        chosenSettings = upstairsTeachers;
        UpdateDisplay();
    }

    public void ButtonPressed(string name)
    {
        if (name.Equals("Left"))
        {
            if (currentChallenge > 0)
                currentChallenge--;
            else
                currentChallenge = challengeNames.Length - 1;
        }
        else if (name.Equals("Right"))
        {
            if (currentChallenge < challengeNames.Length - 1)
                currentChallenge++;
            else
                currentChallenge = 0;

        }
        else if (name.Equals("Start"))
        {
            if (currentChallenge == 4 && PlayerData.Stars != 3) { return; }

            GameManager.challengeIndex = currentChallenge;
            GameManager.challengeSettings = chosenSettings;
            PlayerData.SetNight(7);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
        
        switch (currentChallenge)
        {
            case 0:
                chosenSettings = upstairsTeachers;
                break;
            case 1:
                chosenSettings = soundIsYourFriend;
                break;
            case 2:
                chosenSettings = besties;
                break;
            case 3:
                chosenSettings = theMiddle;
                break;
            case 4:
                chosenSettings = somethingsMissing;
                break;
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        SaveData data = PlayerData.GetPlayerData();
        
        // getting name of challenge
        string chalName = challengeNames[currentChallenge];
        challengeDisplay.SetText(chalName);

        // checking if player completed challenge
        if (data.completedChallenges[currentChallenge])
            challengeDisplay.color = completedChallenge;
        else
            challengeDisplay.color = incompletedChallenge;

        // modifying teacher portraits
        for (int i = 0; i < chosenSettings.Length; i++)
        {
            if (chosenSettings[i].activity > 0)
                teacherPortraits[i].color = Color.white;
            else
                teacherPortraits[i].color = new Color(1f, 1f, 1f, .5f);
        }

        // locking "something's missing..." until prerequisites are met
        if (currentChallenge == 4 && data.stars != 3)
        {
            startBtn.transform.Find("Arrows").GetComponent<TextMeshProUGUI>().SetText("");
            startBtn.color = new Color(1f, 1f, 1f, .5f);
        }
        else
        {
            startBtn.transform.Find("Arrows").GetComponent<TextMeshProUGUI>().SetText(">>");
            startBtn.color = Color.white;
        }
    }
}
