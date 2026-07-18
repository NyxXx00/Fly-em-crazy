using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoScene : MonoBehaviour {
    public VideoPlayer videoPlayer;
    public string nextScene;

    private void Start() {
        videoPlayer.loopPointReached += EndReached;
    }

    private void EndReached(VideoPlayer vp) {
        SceneManager.LoadScene(nextScene);
    }
}
