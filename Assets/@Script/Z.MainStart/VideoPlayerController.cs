using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;      // Video Player ������Ʈ
    public VideoClip[] videoClips;       // �� ���� ���� Ŭ��
    public RawImage rawImage;            // ������ ǥ���� UI
    public RenderTexture renderTexture;  // ���� ��¿� RenderTexture

    private int currentVideoIndex = 0;   // ���� ��� ���� ���� �ε���

    void Start()
    {
        if (videoClips.Length == 2 && rawImage != null && renderTexture != null)
        {
            // RenderTexture�� VideoPlayer�� ������� ����
            videoPlayer.targetTexture = renderTexture;
            rawImage.texture = renderTexture;

            // ù ��° ���� ���� �� ���
            videoPlayer.clip = videoClips[currentVideoIndex];
            videoPlayer.Play();

            // ���� ������ ���� �������� ��ȯ
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogError("���� Ŭ�� 2��, RawImage, RenderTexture�� �����ϼ���!");
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // ���� ���� �ε����� ����
        currentVideoIndex = (currentVideoIndex + 1) % 2;
        videoPlayer.clip = videoClips[currentVideoIndex];
        videoPlayer.Play();
    }
}
