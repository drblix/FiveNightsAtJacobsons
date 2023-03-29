using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private const float RESET_HOLD_TIME = 2f;
    private readonly string[] buttonNames = { "NewGame", "Continue", "Sixth", "Custom", "Back", "Credits", "CreditsBack", "StartCustom" };

    [SerializeField] private Light flashingLight;
    [SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private AudioSource glitchSource;
    [SerializeField] private Transform[] animatronics;
    [SerializeField] private AudioClip[] glitchSfxs;

    [Header("Title Page UI References")]

    [SerializeField] private Image delFill, newspaper, blackFade;
    [SerializeField] private TextMeshProUGUI continueBtn, nightDisplay;
    [SerializeField] private GameObject sixthNight, customNight, mainContainer, customContainer, creditsContainer;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TextMeshProUGUI[] activityNums;

    private ChromaticAberration ca;
    private Grain gr;

    private float resetTimer = 0f;
    private bool inCutscene = false;
    public static int[] animActivities = new int[5];

    private void Start() 
    {
        postProcessVolume.profile.TryGetSettings(out ca);
        postProcessVolume.profile.TryGetSettings(out gr);

        LoadOptions();

        StartCoroutine(LightLoop());
        StartCoroutine(GlitchLoop());
    }

    private void Update()
    {
        if (!inCutscene)
            ResetDataTimer();
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Application.Quit();
    }

    private void ResetDataTimer()
    {
        if (Keyboard.current.deleteKey.isPressed && mainContainer.activeInHierarchy)
        {
            resetTimer += Time.deltaTime;

            if (resetTimer > RESET_HOLD_TIME)
            {
                PlayerData.WipeData();
                LoadOptions();
                resetTimer = 0f;
            }
        }
        else
        {
            resetTimer = 0f;
        }

        delFill.fillAmount = resetTimer / RESET_HOLD_TIME;
    }

    public void ButtonPressed(string name)
    {
        if (name.Equals(buttonNames[0]))
        {
            PlayerData.SetNight(1);
            StartCoroutine(NewspaperCutscene());
        }
        else if (name.Equals(buttonNames[1]) && PlayerData.Night != 1)
        {
            PlayerData.SetNight(Mathf.Clamp(PlayerData.Night, 2, 5));
            SceneManager.LoadScene(1);
        }
        else if (name.Equals(buttonNames[2]))
        {
            PlayerData.SetNight(6);
            GameManager.sixthNight = true;
            SceneManager.LoadScene(1);
        }
        else if (name.Equals(buttonNames[3]))
        {
            StartCoroutine(FadeToFrom(mainContainer, customContainer));
        }
        else if (name.Equals(buttonNames[4]))
        {
            StartCoroutine(FadeToFrom(customContainer, mainContainer));
        }
        else if (name.Equals(buttonNames[5]))
        {
            StartCoroutine(FadeToFrom(mainContainer, creditsContainer));
        }
        else if (name.Equals(buttonNames[6]))
        {
            StartCoroutine(FadeToFrom(creditsContainer, mainContainer));
        }
        else if (name.Equals(buttonNames[7]))
        {
            int count = 0;
            foreach (int a in animActivities)
                if (a == 20)
                    count++;

            GameManager.twentyMode = count == 5;
            PlayerData.SetNight(7);
            SceneManager.LoadScene(1);
        }
    }

    private void LoadOptions()
    {
        SaveData data = PlayerData.GetPlayerData();
        
        if (data.night <= 1)
        {
            continueBtn.color = new Color(1, 1, 1, 0.372f);
            nightDisplay.SetText("");
            continueBtn.transform.Find("Arrows").GetComponent<TextMeshProUGUI>().SetText("");
        }
        else
        {
            continueBtn.color = Color.white;
            nightDisplay.SetText("Night " + Mathf.Clamp(data.night, 2, 5));
            continueBtn.transform.Find("Arrows").GetComponent<TextMeshProUGUI>().SetText(">>");
        }

        for (int i = 0; i < stars.Length; i++)
            stars[i].SetActive(false);
        
        for (int i = 0; i < data.stars; i++)
            stars[i].SetActive(true);

        sixthNight.SetActive(data.unlockedSixth);
        customNight.SetActive(data.unlockedCustom);
    }

    // Disables all poses the animatronic has and enables the provided one
    private void SetAnimatronicPose(Transform animatronic, int pose)
    {
        foreach (Transform child in animatronic)
        {
            child.gameObject.SetActive(false);
        }

        if (pose != 0)
        {
            animatronic.Find("Glitch" + pose).gameObject.SetActive(true);
        }
        else
        {
            animatronic.Find("Default").gameObject.SetActive(true);
        }
    }

    private IEnumerator GlitchLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 6f));

            // 50% chance of a glitch event happening
            if (Random.Range(1, 3) == 2)
            {   
                // Causes the CA effect every glitch
                ca.intensity.value = 1;
                StartCoroutine(CAToZero());

                // Randomly chooses a glitch sfx and plays it
                glitchSource.clip = glitchSfxs[Random.Range(0, glitchSfxs.Length)];
                glitchSource.Play();

                // Picking a random glitch pose for every animatronic
                for (int i = 0; i < animatronics.Length; i++)
                {
                    if (animatronics[i].gameObject.activeInHierarchy)
                    {
                        SetAnimatronicPose(animatronics[i], Random.Range(0, 3));
                        yield return new WaitForSeconds(Random.Range(.02f, .15f));
                    }
                }

                // Returning all animatronics to their default pose
                for (int i = 0; i < animatronics.Length; i++)
                {
                    SetAnimatronicPose(animatronics[i], 0);
                    yield return new WaitForSeconds(Random.Range(.02f, .15f));
                }

                if (inCutscene)
                    break;
            }
        }
    }

    // Flickering light loop
    private IEnumerator LightLoop()
    {
        while (true)
        {
            flashingLight.enabled = false;
            yield return new WaitForSeconds(Random.Range(0.02f, .3f));

            if (inCutscene)
                break;

            flashingLight.enabled = true;
            yield return new WaitForSeconds(Random.Range(.25f, 1.8f));

            if (inCutscene)
                break;
        }
    }

    // Lerps the CA intensity back to zero
    private IEnumerator CAToZero()
    {
        float num = 1f;

        while (num > 0f)
        {
            if (inCutscene)
                break;
            
            num -= Time.deltaTime * 2.25f;
            ca.intensity.value = num;
            yield return new WaitForEndOfFrame();
        }

        ca.intensity.value = 0f;
    }

    private IEnumerator NewspaperCutscene()
    {
        newspaper.gameObject.SetActive(true);
        inCutscene = true;
        glitchSource.volume = 0f;

        float timer = 0f;
        while (timer < 4f)
        {
            float quotient = timer / 4f;
            newspaper.color = new Color(1, 1, 1, quotient);
            gr.intensity.value = 1f - quotient;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        newspaper.color = Color.white;
        gr.intensity.value = 0;

        timer = 0f;
        while (timer < 12f)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.anyKey.wasPressedThisFrame)
                break;


            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(1);
    }

    // Identifier = First character is animatronic, second is add one or subtract one
    // Example = "W+" : This would add one to Wolf's activity
    public void ActivityChange(string identifier)
    {
        if (!identifier.Equals("TWENTYMODE"))
        {
            char animatronic = identifier[0];
            int change = identifier[1] == '+' ? 1 : -1;

            switch (animatronic)
            {
                case 'W':
                    animActivities[0] += change;
                    animActivities[0] = Mathf.Clamp(animActivities[0], 0, 20);
                    activityNums[0].SetText(animActivities[0].ToString());
                    break;
                case 'M':
                    animActivities[1] += change;
                    animActivities[1] = Mathf.Clamp(animActivities[1], 0, 20);
                    activityNums[1].SetText(animActivities[1].ToString());
                    break;
                case 'Z':
                    animActivities[2] += change;
                    animActivities[2] = Mathf.Clamp(animActivities[2], 0, 20);
                    activityNums[2].SetText(animActivities[2].ToString());
                    break;
                case 'R':
                    animActivities[3] += change;
                    animActivities[3] = Mathf.Clamp(animActivities[3], 0, 20);
                    activityNums[3].SetText(animActivities[3].ToString());
                    break;
                case 'F':
                    animActivities[4] += change;
                    animActivities[4] = Mathf.Clamp(animActivities[4], 0, 20);
                    activityNums[4].SetText(animActivities[4].ToString());
                    break;
            }
        }
        else
        {
            for (int i = 0; i < animActivities.Length; i++)
            {
                animActivities[i] = 20;
                activityNums[i].SetText("20");
            }
        }
    }

    private IEnumerator FadeToFrom(GameObject from, GameObject to)
    {
        // "to" is true if going to the custom menu
        const float LENGTH = 1.5f;

        blackFade.gameObject.SetActive(true);
        float alpha = 0f;
        float timer = 0f;

        while (timer < LENGTH)
        {
            alpha = Mathf.Lerp(0f, 1f, timer / LENGTH);
            blackFade.color = new Color(0, 0, 0, alpha);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        blackFade.color = Color.black;
        timer = 0f;

        if (from == mainContainer && to == creditsContainer)
        {
            animatronics[0].gameObject.SetActive(false);
            animatronics[1].gameObject.SetActive(false);
            animatronics[2].gameObject.SetActive(false);
            animatronics[3].gameObject.SetActive(true);
            animatronics[4].gameObject.SetActive(true);
        }
        else if (from == creditsContainer && to == mainContainer)
        {
            animatronics[0].gameObject.SetActive(true);
            animatronics[1].gameObject.SetActive(true);
            animatronics[2].gameObject.SetActive(true);
            animatronics[3].gameObject.SetActive(false);
            animatronics[4].gameObject.SetActive(false);
        }

        from.SetActive(false);
        to.SetActive(true);

        while (timer < LENGTH)
        {
            alpha = 1f - Mathf.Lerp(0f, 1f, timer / LENGTH);
            blackFade.color = new Color(0, 0, 0, alpha);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        blackFade.color = Color.clear;
        blackFade.gameObject.SetActive(false);
    }
}