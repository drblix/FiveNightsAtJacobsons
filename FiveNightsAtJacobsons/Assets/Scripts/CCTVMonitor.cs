using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CCTVMonitor : MonoBehaviour
{
    private const string PUPPET_CAM = "4";
    private const string DOYLE_CAM = "5";
    
    public bool camerasOpen { get; set; }

    private Player player;
    private GameManager gameManager;

    [Header("CCTV Variables")]

    [SerializeField]
    private AudioSource[] cctvSources;

    [SerializeField]
    private AudioClip[] cctvClips;

    [SerializeField]
    private CCTVCam[] cctvCams;

    [SerializeField]
    private TextMeshProUGUI camName;

    [Header("Gameobjects")]

    [SerializeField]
    private GameObject cctvObj;

    [SerializeField]
    private GameObject puppetControls;

    [SerializeField]
    private GameObject camDisconnected;

    [SerializeField]
    private RawImage camDisplay;

    [Header("Animation")]

    [SerializeField]
    private Animator monitorAnimator;

    [SerializeField]
    private Sprite idleCam;

    private float ambienceTimer = 0f;
    private float randWait;
    private bool animatronicMoving = false;
    private string currentCam = "";


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>();
        randWait = Random.Range(15f, 30f);
        // sets default cam location on start
        // CamPressed("2A");
    }

    private void Update()
    {
        if (camerasOpen)
        {
            // randomly plays ambience after a certain time threshold
            if (ambienceTimer > randWait)
            {
                cctvSources[1].clip = cctvClips[Random.Range(0, cctvClips.Length)];
                cctvSources[1].Play();

                ambienceTimer = 0f;
                randWait = Random.Range(15f, 30f);
            }

            ambienceTimer += Time.deltaTime;
        }
    }

    public void ToggleCams(bool bypass)
    {
        // returns early if animation to open/close monitor is currently playing
        if (!bypass) {
            if (monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("MonitorUp") || monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("MonitorDown") || gameManager.gameOver || PowerManager.powerEmpty)
                return;
        }

        // sets camera open to the opposite of cctvObj's activity in hierarchy
        camerasOpen = !cctvObj.activeInHierarchy;
        player.canInteract = !camerasOpen;

        // setting volume of objects in office
        cctvSources[4].volume = camerasOpen ? .5f : 1f;
        cctvSources[5].volume = camerasOpen ? .3f : .65f;
        cctvSources[6].volume = camerasOpen ? .225f : .6f;

        // plays monitor opening sound if not already playing
        if (!cctvSources[3].isPlaying) { cctvSources[3].Play(); }

        // if cameras are closed and cam ambience is playing, stop it
        if (!camerasOpen && cctvSources[1].isPlaying) { cctvSources[1].Stop(); }

        StartCoroutine(ToggleMonitor());

        // loops through each camera and sets its activity state depending on
        // whether the cams are currently open and the active texture
        // matches the object's name
        // also disables the whirring audio if the camera is disabled
        for (int i = 0; i < cctvCams.Length; i++)
        {
            bool state = camDisplay.texture.name.Equals(cctvCams[i].transform.name) && camerasOpen;
            cctvCams[i].enabled = state;
            cctvCams[i].GetComponent<Camera>().enabled = state;
            cctvCams[i].GetComponent<Light>().enabled = state;
            if (!state)
                cctvCams[i].GetComponent<AudioSource>().Stop();
        }
    }

    public void CamPressed(string name)
    {
        // randomizes pitch and plays beep noise
        cctvSources[0].pitch = Random.Range(0.9f, 1.1f);
        cctvSources[0].Play();

        for (int i = 0; i < cctvCams.Length; i++)
        {
            // if is cam that was selected, enabled components
            if (cctvCams[i].name.Equals(name))
            {
                currentCam = name;

                // enables camera's script, animator, and sets render texture
                cctvCams[i].enabled = true;
                cctvCams[i].GetComponent<Camera>().enabled = true;
                cctvCams[i].GetComponent<Light>().enabled = true;
                cctvCams[i].camAnimator.enabled = true;
                camDisplay.texture = cctvCams[i].camTexture;
                camName.SetText(cctvCams[i].camName);

                // shows puppet controls if it's the puppet cam
                puppetControls.SetActive(cctvCams[i].name.Equals(PUPPET_CAM));
                
                // prevents overriding
                if (!animatronicMoving)
                    camDisconnected.SetActive(cctvCams[i].name.Equals(DOYLE_CAM));
            }
            else
            {
                // disables all components on camera
                cctvCams[i].enabled = false;
                cctvCams[i].GetComponent<Camera>().enabled = false;
                cctvCams[i].GetComponent<Light>().enabled = false;
                cctvCams[i].GetComponent<AudioSource>().Stop();
                cctvCams[i].camAnimator.enabled = false;
                cctvCams[i].camAnimator.GetComponent<Image>().sprite = idleCam;
            }
        }
    }

    public IEnumerator ToggleMonitor()
    {
        // the whole point of this is to just stop the camera from being spammed
        // and to give that delay before the cctv stuff is shown
        if (camerasOpen)
        {
            monitorAnimator.gameObject.SetActive(true);
            monitorAnimator.Play("MonitorUp");
            cctvSources[2].Play();

            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => !monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("MonitorUp"));

            cctvObj.SetActive(camerasOpen);
            monitorAnimator.gameObject.SetActive(false);
        }
        else
        {
            monitorAnimator.gameObject.SetActive(true);
            monitorAnimator.Play("MonitorDown");
            cctvSources[2].Stop();
            cctvObj.SetActive(camerasOpen);

            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => !monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("MonitorDown"));

            monitorAnimator.gameObject.SetActive(false);
        }
    }

    public IEnumerator DisconnectCams(float dur)
    {
        animatronicMoving = true;
        player.canUseFlashlight = false;
        camDisconnected.SetActive(true);
        yield return new WaitForSeconds(dur);

        if (!currentCam.Equals(DOYLE_CAM))
            camDisconnected.SetActive(false);
        
        animatronicMoving = false;
        player.canUseFlashlight = true;
    }
}
