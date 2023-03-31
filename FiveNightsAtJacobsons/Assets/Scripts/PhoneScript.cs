using System.Collections;
using UnityEngine;

public class PhoneScript : MonoBehaviour
{
    public static bool captionsEnabled = false;

    private AudioSource phoneRing;
    [SerializeField] private ClosedCaption closedCaption;
    [SerializeField] private AudioSource keyPress, phoneSpeaker;
    [SerializeField] private GameObject muteButton;
    [SerializeField] private AudioClip[] phoneLines;

    [SerializeField] private bool shouldPlay = true;
    private string enteredCode = "";

    [Header("Captioned Calls")]
    [SerializeField] private CaptionedAudio[] night1;
    [SerializeField] private CaptionedAudio[] night2;
    [SerializeField] private CaptionedAudio[] night3;
    [SerializeField] private CaptionedAudio[] night4;
    [SerializeField] private CaptionedAudio[] night5;
    private CaptionedAudio[] captionList;

    private void Awake() 
    {
        phoneRing = GetComponent<AudioSource>();

        if (PlayerData.Night - 1 < phoneLines.Length && shouldPlay) 
        {
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

    public void MuteCall()
    {
        phoneRing.Stop();
        phoneSpeaker.Stop();
        phoneRing.enabled = false;
        phoneSpeaker.enabled = false;
        muteButton.SetActive(false);
        closedCaption.gameObject.SetActive(false);
    }

    private IEnumerator PhoneSequence()
    {
        yield return new WaitForSeconds(5f);
        phoneRing.Play();
        muteButton.SetActive(true);

        if (captionsEnabled)
        {
            switch (PlayerData.Night)
            {
                case 1:
                    captionList = night1;
                    break;
                case 2:
                    captionList = night2;
                    break;
                case 3:
                    captionList = night3;
                    break;
                case 4:
                    captionList = night4;
                    break;
                case 5:
                    captionList = night5;
                    break;
            }

            closedCaption.gameObject.SetActive(true);
            closedCaption.SetCaption("*phone ringing*");
            closedCaption.DisplayCaption();
        }


        yield return new WaitForSeconds(phoneRing.clip.length + .8f);

        phoneSpeaker.Play();

        // loop to display captions for phone call
        if (captionsEnabled)
        {
            float endStamp;

            for (int i = 0; i < captionList.Length; i++)
            {
                if (!closedCaption.gameObject.activeInHierarchy) { break; }

                endStamp = captionList[i].endStamp;
                closedCaption.SetCaption(captionList[i].caption);
                closedCaption.DisplayCaption();
                yield return new WaitUntil(() => phoneSpeaker.time >= endStamp);
            }
            
            yield return new WaitWhile(() => phoneSpeaker.isPlaying);
            closedCaption.gameObject.SetActive(false);
        }

        yield return new WaitWhile(() => phoneSpeaker.isPlaying);

        muteButton.SetActive(false);
    }
}
