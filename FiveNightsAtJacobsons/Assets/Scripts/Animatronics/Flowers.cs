using System.Collections;
using UnityEngine;

public class Flowers : MonoBehaviour
{

    private const float MOVE_THRESHOLD = 27f;

    private GameManager gameManager;

    [SerializeField] [Range(0, 20)] private int activity = 20;
    private float moveTimer = 0f;

    private CCTVMonitor cctvMonitor;

    [SerializeField] private AudioSource flowersVent;

    private int phase = 1;

    private bool attacking = false;

    private void Awake()
    {
        cctvMonitor = FindObjectOfType<CCTVMonitor>();
        gameManager = FindObjectOfType<GameManager>();

        gameManager.applySettings.AddListener(SetSettings);
        gameManager.gameOverEvent.AddListener(() => enabled = false);
    }

    private void Update() 
    {
        if (cctvMonitor.camerasOpen || attacking) { return; }
        moveTimer += Time.deltaTime;

        if (moveTimer > MOVE_THRESHOLD) {
            moveTimer = 0f;

            if (GameManager.DoMoveRoll(activity)) {
                phase++;
                StartCoroutine(cctvMonitor.DisconnectCams(1f));

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
    }

    private void SelectPhase()
    {
        string objName = "Stage" + phase;

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        transform.Find(objName).gameObject.SetActive(true);
    }

    public void SetSettings(AnimatronicSettings settings) {
        if (settings.animatronicName.ToString().Equals(transform.name)) {
            activity = settings.activity;
            moveTimer = 0f;
        }

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
