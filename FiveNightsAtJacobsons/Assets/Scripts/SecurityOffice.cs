using System.Collections;
using UnityEngine;

public class SecurityOffice : MonoBehaviour
{
    // true = closed; false = open
    private bool[] doorStates = new bool[2];
    public bool LeftVentClosed { get { return doorStates[0]; } }
    public bool RightVentClosed { get { return doorStates[1]; } }

    private bool[] ventLights = new bool[2];

    [SerializeField]
    private Animator leftVentAnimator;
    [SerializeField]
    private Animator rightVentAnimator;

    [SerializeField]
    private AudioSource[] doorSounds;

    [SerializeField]
    private MeshRenderer lightButtonL;
    [SerializeField]
    private MeshRenderer lightButtonR;

    [SerializeField]
    private Light ventLightR;
    [SerializeField]
    private Light ventLightL;

    private bool canUseR = true;
    private bool canUseL = true;


    public void ToggleButton(GameObject obj)
    {
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
                anim = leftVentAnimator;
            }
            else if (obj.name.EndsWith('R') && canUseR)
            {
                canUseR = false;
                StartCoroutine(DoorCooldown(0));
                rightVentAnimator.SetBool("Open", !rightVentAnimator.GetBool("Open"));
                doorSounds[1].Play();
                doorStates[1] = !rightVentAnimator.GetBool("Open");
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
        else if (obj.name.Contains("Light"))
        {
            // if the object is a light get the renderer and change the color
            // and start playing sound if haven't already
            MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();
            objRenderer.material.SetColor("_Color", Color.white);
            objRenderer.material.SetColor("_EmissionColor", Color.white * .5f);

            if (obj.name.EndsWith('L') && !doorSounds[2].isPlaying)
            {
                ventLightL.enabled = true;
                doorSounds[2].Play();
            }
            else if (obj.name.EndsWith('R') && !doorSounds[3].isPlaying)
            {
                ventLightR.enabled = true;
                doorSounds[3].Play();
            }            
        }
    }

    public void DisableLights()
    {
        // resets all light material properties and stops sounds
        lightButtonR.material.SetColor("_Color", Color.gray);
        lightButtonR.material.SetColor("_EmissionColor", Color.black);
        lightButtonL.material.SetColor("_Color", Color.gray);
        lightButtonL.material.SetColor("_EmissionColor", Color.black);
        ventLightR.enabled = false;
        ventLightL.enabled = false;
        doorSounds[2].Stop();
        doorSounds[3].Stop();
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
}
