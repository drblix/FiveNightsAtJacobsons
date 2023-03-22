using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SecurityOffice : MonoBehaviour
{
    private Player player;
    private GameManager gameManager;
    private CCTVMonitor cctvMonitor;

    public UnityEvent shockEvent;

    // true = closed; false = open
    private bool[] doorStates = new bool[2];
    public bool LeftVentClosed { get { return doorStates[0]; } }
    public bool RightVentClosed { get { return doorStates[1]; } }

    public bool[] PowerStates { get; private set; } = new bool[6];

    [SerializeField] private Animator leftVentAnimator;
    [SerializeField] private Animator rightVentAnimator;

    [SerializeField] private AudioSource[] doorSounds;
    [SerializeField] private MeshRenderer lightButtonL;
    [SerializeField] private MeshRenderer lightButtonR;
    [SerializeField] private MeshRenderer doorButtonR;
    [SerializeField] private MeshRenderer doorButtonL;

    [SerializeField] private MeshRenderer blackPlaneR;
    [SerializeField] private MeshRenderer blackPlaneL;

    [SerializeField] private Light ventLightR;
    [SerializeField] private Light ventLightL;
    [SerializeField] private Light flashlight;


    private bool canUseR = true;
    private bool canUseL = true;


    private void Awake() {
        player = GetComponent<Player>();
        gameManager = FindObjectOfType<GameManager>();
        cctvMonitor = FindObjectOfType<CCTVMonitor>();

        gameManager.gameOverEvent.AddListener(() => {
            DisableLights();
            enabled = false;
        });
    }

    private void Update() 
    {
        // Sets power states if these are enabled
        PowerStates[4] = flashlight.enabled;
        PowerStates[5] = cctvMonitor.camerasOpen;
    }

    public void ToggleButton(GameObject obj)
    {
        if (PowerManager.powerEmpty) { return; }

        if (obj.name.Contains("Door"))
        {   
            Animator anim = null;

            if (!canUseL || !canUseR) { return; }
            
            // if object is a door and is left one or right one specifically,
            // toggles animation boolean, plays audio, and modifies state in array
            if (obj.name.EndsWith('L') && canUseL)
            {
                canUseL = false;
                StartCoroutine(DoorCooldown(1));
                leftVentAnimator.SetBool("Open", !leftVentAnimator.GetBool("Open"));
                doorSounds[0].Play();
                doorStates[0] = !leftVentAnimator.GetBool("Open");
                PowerStates[0] = doorStates[0];
                anim = leftVentAnimator;
            }
            else if (obj.name.EndsWith('R') && canUseR)
            {
                canUseR = false;
                StartCoroutine(DoorCooldown(0));
                rightVentAnimator.SetBool("Open", !rightVentAnimator.GetBool("Open"));
                doorSounds[1].Play();
                doorStates[1] = !rightVentAnimator.GetBool("Open");
                PowerStates[1] = doorStates[1];
                anim = rightVentAnimator;
            }

            // gets the mesh renderer of the object and changes the color
            // depending on open state
            MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();
            if (anim.GetBool("Open"))
            {
                objRenderer.material.SetColor("_Color", Color.red);
                objRenderer.material.SetColor("_EmissionColor", Color.red * .5f);
            }
            else
            {
                objRenderer.material.SetColor("_Color", Color.green);
                objRenderer.material.SetColor("_EmissionColor", Color.green * .5f);
            }
        }
        else if (obj.name.Contains("Light") && player.canUseVentLight)
        {
            // if the object is a light get the renderer and change the color
            // and start playing sound if haven't already
            MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();
            objRenderer.material.SetColor("_Color", Color.white);
            objRenderer.material.SetColor("_EmissionColor", Color.white * .5f);

            if (obj.name.EndsWith('L') && !doorSounds[2].isPlaying)
            {
                PowerStates[2] = true;
                ventLightL.enabled = true;
                blackPlaneL.enabled = false;
                doorSounds[2].Play();
            }
            else if (obj.name.EndsWith('R') && !doorSounds[3].isPlaying)
            {
                PowerStates[3] = true;
                ventLightR.enabled = true;
                blackPlaneR.enabled = false;
                doorSounds[3].Play();
            }            
        }
        else if (obj.CompareTag("ShockButton"))
        {
            obj.GetComponent<ParticleSystem>().Emit(Random.Range(10, 30));
            shockEvent.Invoke();
        }

        Debug.Log(obj.name);
    }

    public void DisableLights()
    {
        // resets all light material properties and stops sounds
        lightButtonR.material.SetColor("_Color", Color.gray);
        lightButtonR.material.SetColor("_EmissionColor", Color.black);
        lightButtonL.material.SetColor("_Color", Color.gray);
        lightButtonL.material.SetColor("_EmissionColor", Color.black);

        ventLightR.enabled = false;
        blackPlaneR.enabled = true;
        ventLightL.enabled = false;
        blackPlaneL.enabled = true;

        doorSounds[2].Stop();
        doorSounds[3].Stop();

        PowerStates[2] = PowerStates[3] = false;
    }

    // 0 == right; 1 == left
    private IEnumerator DoorCooldown(int doorNum)
    {
        yield return new WaitForSeconds(1f);
        if (doorNum == 0)
        {
            canUseR = true;
        }
        else if (doorNum == 1)
        {
            canUseL = true;
        }
    }
    
    public void DisableEverything()
    {
        // Disables all lights
        DisableLights();

        // Opens all doors
        doorStates[0] = false;
        doorStates[1] = false;
        doorSounds[0].Play();
        doorSounds[1].Play();
        leftVentAnimator.SetBool("Open", true);
        rightVentAnimator.SetBool("Open", true);

        // Sets all buttons to pitch black
        doorButtonR.material.SetColor("_Color", Color.black);
        doorButtonR.material.SetColor("_EmissionColor", Color.black);
        doorButtonL.material.SetColor("_Color", Color.black);
        doorButtonL.material.SetColor("_EmissionColor", Color.black);
    }
}
