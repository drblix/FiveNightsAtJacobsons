using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PuppetBox : MonoBehaviour
{
    private const float MAX_WINDDOWN_TIME = 120f;

    private GameManager gameManager;
    private CCTVMonitor cctvMonitor;
    private AudioSource stepSource;

    [SerializeField] private GameObject zubek;
    [SerializeField] private AudioClip[] stepClips;
    [SerializeField] private Transform[] runningPoints;
    [SerializeField] private Animator[] cautionAnimators;

    [SerializeField] private Image puppetWheel;

    [Header("ACTIVITY SETTINGS - USE TO CHANGE DIFFICULTY")]

    [SerializeField] private int activity = 0;

    [SerializeField] private float windRate = 3f;

    private bool beingWound = false;

    private float windDownTime;

    // timer before leaving box
    private float timer1 = 0f;

    // timer after leaving box
    private float timer2 = 0f;

    private bool puppetOut = false;
    private bool doingRunSequence = false;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        cctvMonitor = FindObjectOfType<CCTVMonitor>();
        stepSource = GetComponent<AudioSource>();
        puppetWheel.fillAmount = 1;

        gameManager.applySettings.AddListener(SetSettings);
        gameManager.gameOverEvent.AddListener(() => enabled = false);
        transform.position = new Vector3(-44.79f, -0.991f, 55.49f);

        FindObjectOfType<PowerManager>().powerOutEvent.AddListener(() =>
        {
            gameObject.SetActive(false);
            enabled = false;
        });

        SetPose(1);
    }

    private void Update()
    {
        // disables script if activity is 0
        enabled = !(activity == 0);

        // if puppet is out and not game over
        // constantly perform move rolls until the player is killed
        if ((timer1 > windDownTime || puppetOut) && !gameManager.gameOver)
        {
            puppetWheel.fillAmount = 0;
            // move timer for puppet is 8 seconds, meaning
            // every 8 seconds it will attempt to kill the player
            puppetOut = true;

            if (timer2 > 3f)
            {
                timer2 = 0f;
                if (GameManager.DoMoveRoll(activity))
                {
                    if (!doingRunSequence)
                    {
                        doingRunSequence = true;

                        zubek.SetActive(false);
                        StartCoroutine(cctvMonitor.DisconnectCams(1f));
                        StartCoroutine(RunningPhase());
                    }
                }
            }


            timer2 += Time.deltaTime;
        }

        if (activity > 0)
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

            int phase = 1;
            if (timer1 >= windDownTime)
            {
                phase = 4;
                StartCoroutine(cctvMonitor.DisconnectCams(1f));
                //puppetOut = true;
            }
            else if (timer1 > (windDownTime / 1.15f))
            {
                for (int i = 0; i < cautionAnimators.Length; i++)
                    if (cautionAnimators[i].gameObject.activeInHierarchy)
                    {
                        phase = 3;
                        cautionAnimators[i].SetBool("Red", true);
                    }
            }
            else if (timer1 > (windDownTime / 1.6f))
            {
                for (int i = 0; i < cautionAnimators.Length; i++)
                {
                    if (cautionAnimators[i].gameObject.activeInHierarchy)
                    {
                        phase = 2;
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

            if (phase != 4)
            {
                if (!InPose(phase))
                {
                    // Debug.Log("CHANGING TO POSE " + phase);
                    StartCoroutine(cctvMonitor.DisconnectCams(1f));
                    SetPose(phase);
                }
            }
            else
            {
                foreach (Transform child in transform)
                    child.gameObject.SetActive(false);
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

    private void SetPose(int phase)
    {
        foreach (Transform child in transform)
        {
            if (child.name.Equals("Stage" + phase))
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private bool InPose(int phase)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy && child.name.Equals("Stage" + phase))
            {
                return true;
            }
        }

        return false;
    }

    // lambda functions for assignment
    public void SetActivity(int a) => activity = a;

    public void SetSettings(AnimatronicSettings settings)
    {
        if (settings.animatronicName.ToString().Equals(transform.name))
        {
            activity = settings.activity;
            windDownTime = settings.windDownTime;
        }
    }

    private IEnumerator RunningPhase()
    {
        int point = 0;
        transform.position = runningPoints[point].position;

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        Transform running = transform.Find("Running");
        running.gameObject.SetActive(true);

        for (int i = 1; i < 5; i++)
        {
            while (Vector3.Distance(transform.position, runningPoints[i].position) > .01f)
            {
                // 6f
                transform.position = Vector3.MoveTowards(transform.position, runningPoints[i].position, Time.deltaTime * 10f);
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, LookAngle(runningPoints[i]), transform.eulerAngles.z);

                yield return new WaitForEndOfFrame();
            }
        }

        gameManager.PlayerDeath(transform, new Vector3(-0.05f, -2.7f, 0.8f));

        yield return new WaitForEndOfFrame();
    }

    private void PlayStep()
    {
        stepSource.clip = stepClips[Random.Range(0, stepClips.Length)];
        stepSource.pitch = Random.Range(.95f, 1.51f);
        stepSource.Play();
    }

    private float LookAngle(Transform toLook)
    {
        float s1 = toLook.transform.position.x - transform.position.x;
        float s2 = toLook.transform.position.z - transform.position.z;
        return Mathf.Atan2(s1, s2) * Mathf.Rad2Deg;
    }

    public void SetWound(bool state) => beingWound = state;
}