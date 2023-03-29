using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private RawImage staticVideo;
    [SerializeField] private Image holdBar;
    [SerializeField] private AudioSource skillIssue, menuTheme;
    [SerializeField] private AudioDistortionFilter distortionFilter;
    [SerializeField] private GameObject[] animatronicObjs;
    [SerializeField] private TMPro.TextMeshProUGUI holdToLeave; 

    public static GameManager.Animatronic diedTo;

    private float holdTimer = 0f;

    private void Awake() 
    {
        animatronicObjs[(int)diedTo].SetActive(true);
        StartCoroutine(InitialFade());
    }

    private void Update() 
    {
        if (Keyboard.current.anyKey.isPressed || Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer > 2f)
            {
                diedTo = GameManager.Animatronic.Wolf;
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            holdTimer -= Time.deltaTime;
            holdTimer = Mathf.Clamp(holdTimer, 0f, 2f);
        }

        holdBar.fillAmount = Mathf.Lerp(0f, 1f, holdTimer / 2f);
        holdBar.color = Color.Lerp(Color.white, Color.red, holdTimer / 2f);
    }

    private IEnumerator InitialFade()
    {
        yield return new WaitForSeconds(3.25f);

        menuTheme.Play();
        float timer = 0f;

        while (timer < 3.5f)
        {
            distortionFilter.distortionLevel = Mathf.Lerp(.7f, .4f, timer / 3.5f);
            staticVideo.color = new Color(1, 1, 1, 1f - (timer / 3.5f));

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // THE FUNNY!!!
        // .2% chance
        if (Random.Range(1, 501) == 1)
            skillIssue.Play();

        staticVideo.color = Color.clear;

        yield return new WaitForSeconds(3f);

        timer = 0f;
        while (timer < 2.5f)
        {
            holdToLeave.alpha = Mathf.Lerp(0f, 1f, timer / 2.5f);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        holdToLeave.alpha = 1f;
    }
}
