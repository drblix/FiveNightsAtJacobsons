using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneFader : MonoBehaviour
{
    private const float FADE_DURATION = 3.5f;

    [SerializeField]
    private Image black;
    [SerializeField]
    private TextMeshProUGUI nightText;
    [SerializeField]
    private RawImage staticTexture;

    private float timer = 0f;


    private void Update()
    {
        float t = timer / FADE_DURATION;

        float alpha1to0 = Mathf.Lerp(1, 0, t);
        float staticAlpha = Mathf.Lerp(0.217f, 0, t);

        black.color = new Color(0, 0, 0, alpha1to0);
        nightText.color = new Color(255, 255, 255, alpha1to0);
        staticTexture.color = new Color(255, 255, 255, staticAlpha);

        timer += Time.deltaTime;

        if (timer >= FADE_DURATION)
            gameObject.SetActive(false);
    }
}
