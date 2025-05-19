using UnityEngine;
using TMPro;
using System.Collections;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void Show(int damage, Color color)
    {
        text.text = damage.ToString();
        text.color = color;

        transform.forward = Camera.main.transform.forward;

        // 위치 조정 (앞쪽 -1.5f, 위로 0.5f, 오른쪽으로 0.3f)
        Vector3 offset = Camera.main.transform.forward * -1.5f
                       + Camera.main.transform.up * -1f
                       + Camera.main.transform.right * 0.5f;

        transform.position += offset;

        StartCoroutine(FloatUpAndDisable());
    }

    IEnumerator FloatUpAndDisable()
    {
        float duration = 1f;
        float timer = 0f;
        Vector3 startPos = transform.position;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.position = startPos + Vector3.up * timer;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
