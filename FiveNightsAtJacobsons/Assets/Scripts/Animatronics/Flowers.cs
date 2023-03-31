using System.Collections;
using UnityEngine;

public class Flowers : MonoBehaviour
{


    private GameManager gameManager;
    private CCTVMonitor cctvMonitor;
    private SecurityOffice securityOffice;

    [SerializeField] [Range(0, 20)] private int activity = 20;
    [SerializeField] private float moveTimer = 0f;
    [SerializeField] private float moveVariation = 0f;

    [SerializeField] private AudioSource flowersVent, ventSlam;
    [SerializeField] private bool testMode = false;

    private float moveTick = 0f;
    private float currentMoveTime = 0f;

    private int phase = 1;

    private bool attacking = false;

    private void Awake()
    {
        cctvMonitor = FindObjectOfType<CCTVMonitor>();
        gameManager = FindObjectOfType<GameManager>();
        securityOffice = FindObjectOfType<SecurityOffice>();

        gameManager.applySettings.AddListener(SetSettings);
        gameManager.gameOverEvent.AddListener(() => enabled = false);
        FindObjectOfType<PowerManager>().powerOutEvent.AddListener(() => {
            gameObject.SetActive(false);
            enabled = false;
        });
    }

    private void Update() 
    {
        if (cctvMonitor.camerasOpen || attacking) { return; }

        if (moveTick > currentMoveTime || testMode) {
            if (GameManager.DoMoveRoll(activity) || testMode) {
                phase++;
                StartCoroutine(cctvMonitor.DisconnectCams(1f));

                if (phase != 5)
                    SelectPhase(false);
                else 
                {
                    foreach (Transform child in transform)
                        child.gameObject.SetActive(false);
                    StartCoroutine(FlowersAttack());
                    phase = 0;
                }
            }

            moveTick = 0f;
            currentMoveTime = moveTimer + Random.Range(-moveVariation, moveVariation);
        }

        moveTick += Time.deltaTime;
    }

    private void SelectPhase(bool none)
    {
        string objName = "Stage" + phase;

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        if (transform.Find(objName) && !none)
            transform.Find(objName).gameObject.SetActive(true);
    }

    public void SetSettings(AnimatronicSettings settings) {
        if (settings.animatronicName.ToString().Equals(transform.name)) {
            activity = settings.activity;
            moveTimer = settings.moveTimer;
            moveVariation = settings.moveVariation;
            moveTick = 0f;
            currentMoveTime = moveTimer + Random.Range(-moveVariation, moveVariation);
        }

    }

    private IEnumerator FlowersAttack()
    {
        attacking = true;
        SelectPhase(true);
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        flowersVent.Play();
        yield return new WaitForSeconds(flowersVent.clip.length + Random.Range(.75f, 1.5f));
        flowersVent.Stop();

        if (!securityOffice.LeftVentClosed)
        {
            gameManager.PlayerDeath(transform, new Vector3(0f, -3f, 1f));
        }
        else
        {
            yield return new WaitForSeconds(.25f);
            ventSlam.Play();    
            // resets stuff
            attacking = false;
            phase = 1;
            SelectPhase(false);
            StartCoroutine(cctvMonitor.DisconnectCams(1f));
        }
    }
}
