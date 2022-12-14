using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CCTVMonitor : MonoBehaviour
{
    private const string PUPPET_CAM = "3A";

    [HideInInspector]
    public bool camerasOpen = false;

    private Player player;

    [Header("CCTV Variables")]

    [SerializeField]
    private AudioSource[] cctvSources;

    [SerializeField]
    private AudioClip[] cctvClips;

    [SerializeField]
    private CCTVCam[] cctvCams;

    [Header("Gameobjects")]

    [SerializeField]
    private GameObject cctvObj;

    [SerializeField]
    private GameObject puppetControls;

    [SerializeField]
    private RawImage camDisplay;

    [Header("Animation")]

    [SerializeField]
    private Animator monitorAnimator;

    [SerializeField]
    private Sprite idleCam;

    private float ambienceTimer = 0f;
    private float randWait;

    private void Awake()
    {
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

    public void ToggleCams()
    {
        // returns early if animation to open/close monitor is currently playing
        if (monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("MonitorUp") || monitorAnimator.GetCurrentAnimatorStateInfo(0).IsName("MonitorDown"))
            return;

        // sets camera open to the opposite of cctvObj's activity in hierarchy
        camerasOpen = !cctvObj.activeInHierarchy;
        player.canLook = !camerasOpen;
        cctvSources[4].volume = camerasOpen ? .5f : 1f;


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
            if (!state)
            {
                cctvCams[i].GetComponent<AudioSource>().Stop();
            }
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
                // enables camera's script, animator, and sets render texture
                cctvCams[i].enabled = true;
                cctvCams[i].GetComponent<Camera>().enabled = true;
                cctvCams[i].camAnimator.enabled = true;
                camDisplay.texture = cctvCams[i].camTexture;

                // shows puppet controls if it's the puppet cam
                if (cctvCams[i].name.Equals(PUPPET_CAM))
                {
                    puppetControls.SetActive(true);
                }
                else
                {
                    puppetControls.SetActive(false);
                }
            }
            else
            {
                // disables all components on camera
                cctvCams[i].enabled = false;
                cctvCams[i].GetComponent<Camera>().enabled = false;
                cctvCams[i].GetComponent<AudioSource>().Stop();
                cctvCams[i].camAnimator.enabled = false;
                cctvCams[i].camAnimator.GetComponent<Image>().sprite = idleCam;
            }
        }
    }

    private IEnumerator ToggleMonitor()
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

}
