using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingScreen : MonoBehaviour
{
    [SerializeField] private Image blackImg;
    [SerializeField] private RectTransform checkTransform;
    
    private Quaternion checkRotation;
    private float zRotation = 0f;
    private bool reverse = false;

    private void Start() => StartCoroutine(EndingSequence());

    private void Update() 
    {
        checkRotation = Quaternion.Euler(0f, 0f, zRotation);

        if (reverse)
            zRotation -= Time.deltaTime * 2f;
        else
            zRotation += Time.deltaTime * 2f;

        if (zRotation >= 3f && !reverse)
            reverse = true;
        else if (zRotation <= -3f && reverse)
            reverse = false;
        
        checkTransform.rotation = checkRotation;
    }

    private IEnumerator EndingSequence()
    {
        const float FADE_SPEED = 3f;
        float timer = 0f;

        while (timer < FADE_SPEED)
        {
            blackImg.color = Color.Lerp(Color.black, Color.clear, timer / FADE_SPEED);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(10f);

        timer = 0f;
        while (timer < FADE_SPEED)
        {
            blackImg.color = Color.Lerp(Color.clear, Color.black, timer / FADE_SPEED);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (PlayerData.Stars < 1)
            PlayerData.SetStars(1);
        
        PlayerData.SetSixth(true);

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
