using TMPro;
using UnityEngine;

public class DamageTextAnimation : MonoBehaviour
{
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // 텍스트가 위로 올라가게 하기
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * 3f) * 0.5f;

        // 투명도가 서서히 줄어들게 하기
        Color textColor = GetComponent<TextMeshPro>().color;
        textColor.a -= Time.deltaTime; // 서서히 투명해짐
        GetComponent<TextMeshPro>().color = textColor;
    }
}
