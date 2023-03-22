using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFlicker : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites; // array of Sprites to flicker between
    private Image image; // reference to the Image component of the GameObject this script is attached to
    
    private void Start()
    {
        image = GetComponent<Image>(); // get reference to the Image component
        StartCoroutine("FlickerSequence"); // start the flicker sequence
    }

    private IEnumerator FlickerSequence() // coroutine to handle the flickering
    {
        while (true) // loop forever
        {
            image.sprite = sprites[0]; // set the Image's sprite to the first one in the array

            yield return new WaitForSeconds(Random.Range(2f, 4f)); // wait for a random amount of time between 2 and 4 seconds
            image.sprite = sprites[1]; // set the Image's sprite to the second one in the array
            yield return new WaitForSeconds(Random.Range(0.05f, 0.25f)); // wait for a random amount of time between 0.05 and 0.25 seconds
        }
    }

    private void OnEnable() => Start(); // when the script is enabled, start the flicker sequence
    private void OnDisable() => StopCoroutine("FlickerSequence"); // when the script is disabled, stop the flicker sequence
}
