using UnityEngine;

public class AmbianceManager : MonoBehaviour
{
    // Serialized fields that can be set in the inspector
    [SerializeField] private AudioClip[] ambianceClips;
    [SerializeField] private AudioSource ambianceSource, phoneRing, phoneSpeaker;

    // Fields to keep track of timing
    private float playTimer = 0f;
    private float randReq = 0f;

    // Set randReq to a random value between 70 and 240
    private void Awake() => randReq = Random.Range(70f, 240f);

    private void Update()
    {
        // Check if it's time to play a new audio clip
        if (randReq > playTimer && !phoneRing.isPlaying && !phoneSpeaker.isPlaying)
        {
            // Select a random audio clip from the ambianceClips array and play it
            ambianceSource.clip = ambianceClips[Random.Range(0, ambianceClips.Length)];
            ambianceSource.Play();

            // Reset the playTimer and set randReq to a new random value
            playTimer = 0f;
            randReq = Random.Range(70f, 240f);
        }

        // Increment the playTimer based on the time since the last frame
        playTimer += Time.deltaTime;
    }
}
