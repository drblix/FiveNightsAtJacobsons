using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class TrailerScene : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    private void Update() 
    {
        if (videoPlayer.time >= 36f || Keyboard.current.anyKey.wasPressedThisFrame)
        {
            MainMenu.comingFromTrailer = true;
            SceneManager.LoadScene("Menu");
        }
    }
}
