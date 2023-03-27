using System.Collections;
using UnityEngine;

public class PhoneScript : MonoBehaviour
{

    private AudioSource phoneRing;
    [SerializeField] private AudioSource keyPress;
    [SerializeField] private AudioSource phoneSpeaker;

    [SerializeField] private AudioClip[] phoneLines;

    [SerializeField] private bool shouldPlay = true;
    private string enteredCode = "";


    private void Awake() 
    {
        phoneRing = GetComponent<AudioSource>();

        if (PlayerData.Night - 1 < phoneLines.Length && shouldPlay) {
            phoneSpeaker.clip = phoneLines[PlayerData.Night - 1];
            StartCoroutine(PhoneSequence());
        }
    }

    public void KeyPressed(GameObject key)
    {
        if (key.CompareTag("PhoneButton")) {
            switch (key.name)
            {
                case "One":
                    enteredCode += "1";
                    break;
                case "Two":
                    enteredCode += "2";
                    break;
                case "Three":
                    enteredCode += "3";
                    break;
                case "Four":
                    enteredCode += "4";
                    break;
                case "Five":
                    enteredCode += "5";
                    break;
                case "Six":
                    enteredCode += "6";
                    break;
                case "Seven":
                    enteredCode += "7";
                    break;
                case "Eight":
                    enteredCode += "8";
                    break;
                case "Nine":
                    enteredCode += "9";
                    break;
                case "Zero":
                    enteredCode += "0";
                    break;
                case "Hashtag":
                    if (enteredCode.Length > 0)
                        CheckCode();
                    break;
                case "Asterisk":
                    enteredCode = "";
                    break;
            }

            if (enteredCode.Length >= 50)
                enteredCode = "";

            keyPress.Play();
            print(enteredCode);
        }
    }

    private void CheckCode()
    {
        print(enteredCode);
        enteredCode = "";
    }

    private IEnumerator PhoneSequence()
    {
        yield return new WaitForSeconds(5f);
        phoneRing.Play();
        yield return new WaitForSeconds(phoneRing.clip.length + .8f);
        phoneSpeaker.Play();
    }
}
