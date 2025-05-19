using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [SerializeField] private Image fadeImage;            // ���̵�� �̹��� (������ �̹���)
    [SerializeField] private float fadeDuration = 1f;    // ���̵� �ð�

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ���
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }

    // ȭ�� ��Ӱ� (���̵� �ƿ�)
    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(Fade(0f, 1f));
    }

    // ȭ�� ��� (���̵� ��)
    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(1f, 0f));
    }

    // ���̵� ó�� �ڷ�ƾ
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float time = 0f;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration); // ���� �������� ���� ����
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        //���� ���İ����� ����
        fadeImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
