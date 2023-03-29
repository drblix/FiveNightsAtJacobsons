using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class TrailerScene : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    private void Awake() 
    {
        
    }

    private void Update() 
    {
        Debug.Log(videoPlayer.time);   
    }

    // private IEnumerator 
}
