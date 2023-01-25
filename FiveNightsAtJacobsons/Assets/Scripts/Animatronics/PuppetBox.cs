using UnityEngine;
using UnityEngine.UI;

public class PuppetBox : MonoBehaviour
{
    private const float MAX_WINDDOWN_TIME = 120f;

    [SerializeField]
    private Animator[] cautionAnimators;

    [SerializeField]
    private Image puppetWheel;

    [SerializeField]
    private int activity = 0;

    [SerializeField]
    private float windRate = 3f;

    [SerializeField] [Tooltip("Time it takes for the box to winddown to 0; lower values = faster; elements correspond to each night")]
    private float[] windDownTimes = new float[6];

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
        // disables script if activity is 0
        enabled = !(activity == 0);

        // if puppet is out and not game over
        // constantly perform move rolls until the player is killed
        if ((timer1 > windDownTime || puppetOut) && !GameManager.GameOver)
        {
            // move timer for puppet is 8 seconds, meaning
            // every 8 seconds it will attempt to kill the player
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

        PuppetAction();
    }

    private void PuppetAction()
    {
        // runs as long as puppet is still in the box
        if (!puppetOut)
        {
            // if being wound by camera, reduce time elapsed, else keep adding
            if (beingWound)
            {
                timer1 -= Time.deltaTime * windRate;
            }
            else
            {
                timer1 += Time.deltaTime;
            }

            // clamping timer value to prevent errors with the fill calculation
            timer1 = Mathf.Clamp(timer1, 0f, MAX_WINDDOWN_TIME);
            CalculateFill();

            if (timer1 > (windDownTime / 1.15f))
            {
                for (int i = 0; i < cautionAnimators.Length; i++)
                    if (cautionAnimators[i].gameObject.activeInHierarchy)
                    {
                        cautionAnimators[i].SetBool("Red", true);
                    }
            }
            else if (timer1 > (windDownTime / 1.6f))
            {
                for (int i = 0; i < cautionAnimators.Length; i++)
                {
                    if (cautionAnimators[i].gameObject.activeInHierarchy)
                    {
                        cautionAnimators[i].SetBool("Red", false);
                        cautionAnimators[i].SetBool("Yellow", true);
                    }
                }
            }
            else
            {
                for (int i = 0; i < cautionAnimators.Length; i++)
                {
                    if (cautionAnimators[i].gameObject.activeInHierarchy)
                    {
                        cautionAnimators[i].SetBool("Red", false);
                        cautionAnimators[i].SetBool("Yellow", false);
                    }
                }
            }

        }
    }

    private void CalculateFill()
    {
        const int INCREMENTS = 8;
        // calculating fill amount of the UI element
        float fill = 1f - timer1 / windDownTime;

        for (int i = 8; i > 0; i--) 
        {
            float incrementReq = 1f / INCREMENTS * i;
            if (fill <= incrementReq) 
            {   
                puppetWheel.fillAmount = incrementReq;
            }
        }
    }

    // manual choosing of winddown time for balance reasons
    private void DetermineWinddownTime()
    {
        switch (GameManager.CurrentNight)
        {
            case 1:
                windDownTime = windDownTimes[0];
                break;

            case 2:
                windDownTime = windDownTimes[1];
                break;

            case 3:
                windDownTime = windDownTimes[2];
                break;

            case 4:
                windDownTime = windDownTimes[3];
                break;

            case 5:
                windDownTime = windDownTimes[4];
                break;

            // default for night 6 and custom night
            default:
                windDownTime = windDownTimes[5];
                break;
        }
    }

    // lambda functions for assignment
    public void SetActivity(int a) => activity = a;
    public void SetWound(bool state) => beingWound = state;
}