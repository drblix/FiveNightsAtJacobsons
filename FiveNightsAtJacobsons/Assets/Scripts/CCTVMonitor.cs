using UnityEngine;
using UnityEngine.UI;

public class CCTVMonitor : MonoBehaviour
{
    private Player player;

    [SerializeField]
    private AudioSource camClick;

    [SerializeField]
    private RenderTexture[] camTextures;

    [SerializeField]
    private GameObject cctvObj;

    [SerializeField]
    private CCTVCam[] cctvCams;

    [SerializeField]
    private RawImage camDisplay;

    private void Awake() 
    {
        player = FindObjectOfType<Player>();
    }

    public void ToggleCams()
    {
        cctvObj.SetActive(!cctvObj.activeInHierarchy);
        player.canLook = !cctvObj.activeInHierarchy;

        for (int i = 0; i < cctvCams.Length; i++) 
        {
            bool state = camDisplay.texture.name.Equals(cctvCams[i].transform.name) && cctvObj.activeInHierarchy;
            cctvCams[i].inUse = state;
            cctvCams[i].GetComponent<Camera>().enabled = state;
        }
    }

    public void CamPressed(string name)
    {
        camClick.Play();

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
