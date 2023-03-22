using UnityEngine;

public class StepPlayer : MonoBehaviour
{
    private AudioSource stepSource;
    
    [SerializeField] private AudioClip[] stepClips;

    private void Awake() 
    {
        stepSource = GetComponent<AudioSource>();
    }

    private void PlayStep()
    {
        stepSource.clip = stepClips[Random.Range(0, stepClips.Length)];
        stepSource.pitch = Random.Range(.95f, 1.3f);
        stepSource.Play();
    }
}
