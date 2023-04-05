using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsScreen : MonoBehaviour
{
    public static float userVolume = 1f;
    private readonly string[] buttonNames = { "QualityLeft", "QualityRight", "VolumeRight", "VolumeLeft", "Fullscreen" };

    [SerializeField] private TextMeshProUGUI qualitySetting, fullScreen_X;
    [SerializeField] private Image volumeBar;
    [SerializeField] private Gradient redToGreen;

    private void Awake() => UpdateScreen();

    public void ButtonPressed(string name)
    {
        if (name.Equals(buttonNames[0]))
        {
            QualitySettings.DecreaseLevel(true);
        }
        else if (name.Equals(buttonNames[1]))
        {
            QualitySettings.IncreaseLevel(true);
        }
        else if (name.Equals(buttonNames[2]))
        {
            userVolume += .1f;
            userVolume = Mathf.Clamp01(userVolume);
            AudioListener.volume = userVolume;
        }
        else if (name.Equals(buttonNames[3]))
        {
            userVolume -= .1f;
            userVolume = Mathf.Clamp01(userVolume);
            AudioListener.volume = userVolume;
        }
        else if (name.Equals(buttonNames[4]))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        Debug.Log(name);
        UpdateScreen();
    }

    private void UpdateScreen()
    {
        qualitySetting.SetText(QualitySettings.names[QualitySettings.GetQualityLevel()]);
        qualitySetting.color = redToGreen.Evaluate((QualitySettings.GetQualityLevel() + 1) / 6f);

        volumeBar.fillAmount = userVolume / 1f;
        volumeBar.color = redToGreen.Evaluate(userVolume / 1f);
        
        fullScreen_X.enabled = Screen.fullScreen;
    }
}
