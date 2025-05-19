using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [SerializeField] private Image fadeImage;            // 페이드용 이미지 (검은색 이미지)
    [SerializeField] private float fadeDuration = 1f;    // 페이드 시간

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    // 화면 어둡게 (페이드 아웃)
    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(Fade(0f, 1f));
    }

    // 화면 밝게 (페이드 인)
    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(1f, 0f));
    }

    // 페이드 처리 코루틴
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float time = 0f;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration); // 선형 보간으로 투명도 조절
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        //최종 알파값으로 보정
        fadeImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
