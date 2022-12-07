using UnityEngine;

public class SecurityOffice : MonoBehaviour
{
    // true = closed; false = open
    private bool[] doorStates = new bool[3];
    public bool[] DoorStates { get { return doorStates; } }

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


    public void ToggleButton(GameObject obj)
    {
        if (obj.name.Contains("Door"))
        {   
            // if object is a door and is left one or right one specifically,
            // toggles animation boolean, plays audio, and modifies state in array
            if (obj.name.EndsWith('L'))
            {
                leftVentAnimator.SetBool("Open", !leftVentAnimator.GetBool("Open"));
                doorSounds[0].Play();
                doorStates[0] = !leftVentAnimator.GetBool("Open");
            }
            else if (obj.name.EndsWith('R'))
            {
                rightVentAnimator.SetBool("Open", !rightVentAnimator.GetBool("Open"));
                doorSounds[1].Play();
                doorStates[2] = !leftVentAnimator.GetBool("Open");
            }

            // gets the mesh renderer of the object and changes the color
            // depending on open state
            MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();
            if (leftVentAnimator.GetBool("Open"))
            {
                objRenderer.material.SetColor("_Color", Color.red);
                objRenderer.material.SetColor("_EmissionColor", Color.red);
            }
            else
            {
                objRenderer.material.SetColor("_Color", Color.green);
                objRenderer.material.SetColor("_EmissionColor", Color.green);
            }
        }
        else if (obj.name.Contains("Light"))
        {
            // if the object is a light get the renderer and change the color
            // and start playing sound if haven't already
            MeshRenderer objRenderer = obj.GetComponent<MeshRenderer>();
            objRenderer.material.SetColor("_Color", new Color(7f, 113f, 212f));
            objRenderer.material.SetColor("_EmissionColor", Color.white);

            if (obj.name.EndsWith('L') && !doorSounds[2].isPlaying)
            {
                doorSounds[2].Play();
            }
            else if (obj.name.EndsWith('R') && !doorSounds[3].isPlaying)
            {
                doorSounds[2].Play();
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
        doorSounds[2].Stop();
        doorSounds[3].Stop();
    }
}
