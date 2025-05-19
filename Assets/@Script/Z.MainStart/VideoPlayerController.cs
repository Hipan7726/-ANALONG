using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;      // Video Player 컴포넌트
    public VideoClip[] videoClips;       // 두 개의 비디오 클립
    public RawImage rawImage;            // 비디오를 표시할 UI
    public RenderTexture renderTexture;  // 비디오 출력용 RenderTexture

    private int currentVideoIndex = 0;   // 현재 재생 중인 비디오 인덱스

    void Start()
    {
        if (videoClips.Length == 2 && rawImage != null && renderTexture != null)
        {
            // RenderTexture를 VideoPlayer의 출력으로 설정
            videoPlayer.targetTexture = renderTexture;
            rawImage.texture = renderTexture;

            // 첫 번째 비디오 설정 및 재생
            videoPlayer.clip = videoClips[currentVideoIndex];
            videoPlayer.Play();

            // 비디오 끝나면 다음 영상으로 전환
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogError("비디오 클립 2개, RawImage, RenderTexture를 설정하세요!");
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // 다음 비디오 인덱스로 변경
        currentVideoIndex = (currentVideoIndex + 1) % 2;
        videoPlayer.clip = videoClips[currentVideoIndex];
        videoPlayer.Play();
    }
}
