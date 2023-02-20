using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneScript : MonoBehaviour
{
    [SerializeField]
    private AudioSource keyPress;

    private string enteredCode = "";

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
}
