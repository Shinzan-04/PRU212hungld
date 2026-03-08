using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject playButton;
    public GameObject skipButton;

    public string nextScene = "level";

    void Start()
    {
        skipButton.SetActive(true);
        videoPlayer.Play();
        videoPlayer.loopPointReached += EndVideo;
    }

    public void PlayVideo()
    {
        playButton.SetActive(false);
        skipButton.SetActive(true);
        videoPlayer.Play();
    }

    public void SkipVideo()
    {
        LoadGame();
    }

    void EndVideo(VideoPlayer vp)
    {
        LoadGame();
    }

    void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
    }
}