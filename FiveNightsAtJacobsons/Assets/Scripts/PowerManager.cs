using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PowerManager : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;
    private SecurityOffice securityOffice;
    private CCTVMonitor cctvMonitor;

    [HideInInspector] public UnityEvent powerOutEvent;

    [SerializeField] private float divisionConstant = 9.5f;

    [Header("References")]
    [SerializeField] private Transform wolfAnimatronic;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private Light officeLight;
    [SerializeField] private Light spookyLight;
    [SerializeField] private Light jumpscareLight;
    [SerializeField] private TextMeshPro alarmText;
    [SerializeField] private Animator fanBladesAnimator;
    [SerializeField] private GameObject phoneLight;
    [SerializeField] private GameObject powerOutWolf;
    [SerializeField] private GameObject[] uisToDisable;
    [SerializeField] private GameObject[] usageObjs;
    [SerializeField] private AudioSource[] spookySources;

    public static float currentPower { get; private set; } = 100f;
    public static bool powerEmpty = false;
    public static bool cheatMode = false;


    private void Awake() 
    {
        gameManager = FindObjectOfType<GameManager>();
        securityOffice = FindObjectOfType<SecurityOffice>();
        cctvMonitor = FindObjectOfType<CCTVMonitor>();
        player = FindObjectOfType<Player>();
        currentPower = 100f;
        powerEmpty = false;
        cheatMode = false;

        gameManager.gameOverEvent.AddListener(() => enabled = false);

        if (GameManager.twentyMode)
            divisionConstant = 7.85f;
    }

    private void Update() 
    {
        if (currentPower > 0f)
        {
            // Gets total amount of true statements in powerStates array
            int count = SourcesEnabled();

            // Disables all power bars then displays ones that are active up to a max of 3
            for (int i = 0; i < usageObjs.Length; i++)
                usageObjs[i].SetActive(false);
            for (int i = 0; i < Mathf.Clamp(count, 0, 3); i++)
                usageObjs[i].SetActive(true);

            powerText.SetText($"Power Left: {Mathf.Ceil(currentPower)}%");

            // graph for viewing power consumption rates
            // https://www.desmos.com/calculator/weno3vl6rp
            if (!cheatMode)
                currentPower -= (Time.deltaTime / divisionConstant) * (1 + count);
        }
        else if (!powerEmpty)
        {
            powerEmpty = true;
            powerOutEvent.Invoke();
            StartCoroutine(OutOfPowerSequence());
        }
    }

    private IEnumerator OutOfPowerSequence()
    {
        // Stops all audio currently playing
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
            source.Stop();

        // Plays out of power sound
        spookySources[0].Play();

        // Disables lights in office
        officeLight.enabled = alarmText.enabled = false;
        phoneLight.SetActive(false);

        // Disables fan rotating in office
        fanBladesAnimator.enabled = false;

        // Forces player out of cameras
        if (cctvMonitor.camerasOpen)
            cctvMonitor.ToggleCams(true);

        // Disabling everything in office
        securityOffice.DisableEverything();

        foreach (GameObject item in uisToDisable)
            item.SetActive(false);

        yield return new WaitForSeconds(spookySources[0].clip.length + Random.Range(7f, 14f));
        //yield return new WaitForSeconds(2f);
        officeLight.enabled = true;
        officeLight.intensity = 1.4f;

        powerOutWolf.SetActive(true);
        spookyLight.gameObject.SetActive(true);
        spookySources[1].Play();

        StartCoroutine(FlickerLight());
        yield return new WaitForSeconds(Random.Range(12f, 30f));
        //yield return new WaitForSeconds(3f);
        StopCoroutine(FlickerLight());
        officeLight.enabled = false;
        spookySources[1].Stop();
        powerOutWolf.SetActive(false);
        spookyLight.enabled = false;

        yield return new WaitForSeconds(Random.Range(8f, 16f));
        //yield return new WaitForSeconds(3f);
        Debug.Log("DIE!!!!!!");
        jumpscareLight.enabled = true;
        StartCoroutine(player.Jumpscare(wolfAnimatronic, new Vector3(.1f, -2.9f, .6f)));
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            spookyLight.enabled = true;
            yield return new WaitForSeconds(Random.Range(.02f, .25f));
            spookyLight.enabled = false;
            yield return new WaitForSeconds(Random.Range(.075f, .6f));
        }
    }

    private int SourcesEnabled()
    {
        int count = 0;
        foreach (bool b in securityOffice.PowerStates)
            count = b ? count + 1 : count;

        return count;
    }
}
