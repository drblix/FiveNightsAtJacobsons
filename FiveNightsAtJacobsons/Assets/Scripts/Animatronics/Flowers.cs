using System.Collections;
using UnityEngine;

public class Flowers : MonoBehaviour
{
    [SerializeField] [Range(0, 20)]
    private int activity = 20;
    private float moveTimer = 0f;


    private CCTVMonitor cctvMonitor;

    [SerializeField]
    private AudioSource flowersVent;


    private int phase = 1;

    private float phaseDelay = 0f;


    private bool attacking = false;

    private void Awake()
    {
        cctvMonitor = FindObjectOfType<CCTVMonitor>();
        CalcPhaseDelay();
    }

    private void Update() 
    {
        if (cctvMonitor.camerasOpen || attacking) { return; }
        moveTimer += Time.deltaTime;

        if (moveTimer > phaseDelay) {
            phase++;
            moveTimer = 0f;

            if (phase != 5)
                SelectPhase();
            else {
                foreach (Transform child in transform)
                    child.gameObject.SetActive(false);
                StartCoroutine(FlowersAttack());
                phase = 0;
            }
        }
    }


    private void CalcPhaseDelay()
    {
        switch (activity)
        {
            case 0:
                enabled = false;
                break;
            case 1:
                phaseDelay = 100f;
                break;
            case 2:
                phaseDelay = 95f;
                break;
            case 3:
                phaseDelay = 80f;
                break;
            case 4:
                phaseDelay = 75f;
                break;
            case 5:
                phaseDelay = 70f;
                break;
            case 6:
                phaseDelay = 70f;
                break;
            case 7:
                phaseDelay = 60f;
                break;
            case 8:
                phaseDelay = 55f;
                break;
            case 9:
                phaseDelay = 45f;
                break;
            case 10:
                phaseDelay = 40f;
                break;
            case 11:
                phaseDelay = 35f;
                break;
            case 12:
                phaseDelay = 28f;
                break;
            case 13:
                phaseDelay = 25f;
                break;
            default:
                //phaseDelay = 20f;
                phaseDelay = 4f;
                break;
        }
    }

    private void SelectPhase()
    {
        string objName = "Stage" + phase;

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        transform.Find(objName).gameObject.SetActive(true);
    }

    public void SetActivity(int a) {
        activity = a;
        moveTimer = 0f;
        CalcPhaseDelay();
    }

    public void SetSettings(AnimatronicSettings settings) {
        activity = settings.activity;
        phaseDelay = settings.phaseDelay;
    }

    private IEnumerator FlowersAttack()
    {
        attacking = true;
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        flowersVent.Play();
        yield return new WaitForSeconds(Random.Range(2.5f, 5f));
        flowersVent.Stop();
        attacking = false;
    }
}
