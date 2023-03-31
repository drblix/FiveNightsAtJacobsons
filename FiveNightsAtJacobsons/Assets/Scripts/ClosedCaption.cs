using System.Collections;
using UnityEngine;
using TMPro;

public class ClosedCaption : MonoBehaviour
{
    private TextMeshProUGUI captionDisplay;

    private string caption;

    private void Awake() 
    {
        captionDisplay = transform.GetChild(0).GetComponent<TextMeshProUGUI>();    
    }

    public void DisplayCaption() => StartCoroutine(BuildMessage());
    public void SetCaption(string c) => caption = c;

    // gives the effect of the caption gradually appearing in the caption area
    private IEnumerator BuildMessage()
    {
        string buildingStr = "";
        char[] chars = caption.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            buildingStr += chars[i];
            captionDisplay.SetText(buildingStr);
            yield return new WaitForEndOfFrame();
        }
    }
}
