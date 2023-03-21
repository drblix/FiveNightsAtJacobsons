using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private RawImage staticVideo;
    [SerializeField] private AudioSource skillIssue;

    private void Awake() 
    {
        StartCoroutine(InitialFade());
    }

    private void Update() 
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            SceneManager.LoadScene(0);
    }

    private IEnumerator InitialFade()
    {
        yield return new WaitForSeconds(2.5f);

        float timer = 0f;

        while (timer < 3.5f)
        {
            staticVideo.color = new Color(1, 1, 1, 1f - (timer / 3.5f));

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // THE FUNNY!!!
        if (Random.Range(1, 501) == 1)
            skillIssue.Play();

        staticVideo.color = Color.clear;
    }
}
