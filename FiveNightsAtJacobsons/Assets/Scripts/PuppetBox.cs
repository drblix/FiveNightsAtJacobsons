using UnityEngine;
using UnityEngine.UI;

public class PuppetBox : MonoBehaviour
{
    private const float MAX_WINDDOWN_TIME = 120f;

    [SerializeField]
    private Image puppetWheel;

    [SerializeField]
    private int activity = 0;

    [SerializeField]
    private float windRate = 3f;

    private bool beingWound = false;

    private float windDownTime;

    // timer before leaving box
    private float timer1 = 0f;

    // timer after leaving box
    private float timer2 = 0f;

    private bool puppetOut = false;


    private void Awake()
    {
        // winddown time will be fixed between the nights
        // activity will only affect the puppet after being released
        DetermineWinddownTime();
    }

    private void Update()
    {
        enabled = !(activity == 0);

        if ((timer1 > windDownTime || puppetOut) && !GameManager.GameOver)
        {
            puppetOut = true;
            if (timer2 > 8f)
            {
                timer2 = 0f;
                if (GameManager.DoMoveRoll(activity))
                {
                    GameManager.PlayerDeath();
                }
            }

            timer2 += Time.deltaTime;
        }

        if (!puppetOut)
        {
            if (beingWound)
            {
                timer1 -= Time.deltaTime * windRate;
            }
            else
            {
                timer1 += Time.deltaTime;
            }

            timer1 = Mathf.Clamp(timer1, 0f, MAX_WINDDOWN_TIME);
            
            float fill = 1f - timer1 / windDownTime;
            puppetWheel.fillAmount = fill;
        }
    }

    private void DetermineWinddownTime()
    {
        switch (GameManager.CurrentNight)
        {
            case 1:
                windDownTime = 110f;
                break;

            case 2:
                windDownTime = 95f;
                break;

            case 3:
                windDownTime = 80f;
                break;

            case 4:
                windDownTime = 70f;
                break;

            case 5:
                windDownTime = 55f;
                break;

            // default for night 6 and custom night
            default:
                windDownTime = 50f;
                break;
        }
    }

    public void SetActivity(int a) => activity = a;
    public void SetWound(bool state) => beingWound = state;
}