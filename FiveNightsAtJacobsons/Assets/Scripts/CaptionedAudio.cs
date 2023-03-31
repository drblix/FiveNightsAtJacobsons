using UnityEngine;

[CreateAssetMenu(fileName = "CaptionedClip", menuName = "ScriptableObjects/CaptionedClip", order = 1)]
public class CaptionedAudio : ScriptableObject
{
    public float endStamp;

    [TextArea(1, 10)]
    public string caption;
}
