using UnityEngine;
using UnityEngine.UI;

public class CCTVMonitor : MonoBehaviour
{   
    public bool camerasOpen = false;

    private Player player;

    [SerializeField]
    private AudioSource[] cctvSources;

    [SerializeField]
    private AudioClip[] cctvClips;

    [SerializeField]
    private RenderTexture[] camTextures;

    [SerializeField]
    private GameObject cctvObj;

    [SerializeField]
    private CCTVCam[] cctvCams;

    [SerializeField]
    private RawImage camDisplay;

    private float ambienceTimer = 0f;
    private float randWait;

    private void Awake() 
    {
        player = FindObjectOfType<Player>();
        randWait = Random.Range(15f, 30f);
    }

    private void Update() 
    {   
        if (camerasOpen) 
        {
            if (ambienceTimer > randWait) 
            {
                cctvSources[1].clip = cctvClips[Random.Range(0, 1)];
                cctvSources[1].Play();

                ambienceTimer = 0f;
                randWait = Random.Range(15f, 30f);
            }

            ambienceTimer += Time.deltaTime;
        }
    }

    public void ToggleCams()
    {
        camerasOpen = !cctvObj.activeInHierarchy;
        cctvObj.SetActive(camerasOpen);
        player.canLook = !camerasOpen;

        if (!camerasOpen && cctvSources[1].isPlaying) { cctvSources[1].Stop(); }

        if (camerasOpen) 
        {
            cctvSources[2].Play();
        }
        else 
        {
            cctvSources[2].Pause();
        }

        for (int i = 0; i < cctvCams.Length; i++) 
        {
            bool state = camDisplay.texture.name.Equals(cctvCams[i].transform.name) && cctvObj.activeInHierarchy;
            cctvCams[i].inUse = state;
            cctvCams[i].GetComponent<Camera>().enabled = state;
        }
    }

    public void CamPressed(string name)
    {
        cctvSources[0].pitch = Random.Range(0.9f, 1.1f);
        cctvSources[0].Play();

        for (int i = 0; i < camTextures.Length; i++)
        {
            if (camTextures[i].name.Equals(name))
            {
                camDisplay.texture = camTextures[i];
                break;
            }
        }

        for (int i = 0; i < cctvCams.Length; i++) 
        {
            bool state = cctvCams[i].transform.name.Equals(name);
            cctvCams[i].inUse = state;
            cctvCams[i].GetComponent<Camera>().enabled = state;
        }
    }
}
