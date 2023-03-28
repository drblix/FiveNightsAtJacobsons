using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneFader : MonoBehaviour
{
    private const float FADE_DURATION = 3f;

    [SerializeField] private Image black;
    [SerializeField] private TextMeshProUGUI nightText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private RawImage staticTexture;
    private AudioSource staticSource;

    private void Start() 
    {
        staticSource = GetComponent<AudioSource>();

        Debug.Log(PlayerData.Night);
        nightText.SetText($"Night {PlayerData.Night}");
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        
        yield return new WaitForSeconds(1.25f);
        float timer = 0f;

        while (timer < FADE_DURATION)
        {
            float t = timer / FADE_DURATION;

            float alpha1to0 = Mathf.Lerp(1, 0, t);
            float staticAlpha = Mathf.Lerp(0.217f, 0, t);

            black.color = new Color(0, 0, 0, alpha1to0);
            nightText.color = new Color(255, 255, 255, alpha1to0);
            timeText.color = new Color(255, 255, 255, alpha1to0);
            staticTexture.color = new Color(255, 255, 255, staticAlpha);
            staticSource.volume = alpha1to0;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
 
        gameObject.SetActive(false);
        enabled = false;
    }
}
