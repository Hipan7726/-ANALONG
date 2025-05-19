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
        // �ؽ�Ʈ�� ���� �ö󰡰� �ϱ�
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * 3f) * 0.5f;

        // ������ ������ �پ��� �ϱ�
        Color textColor = GetComponent<TextMeshPro>().color;
        textColor.a -= Time.deltaTime; // ������ ��������
        GetComponent<TextMeshPro>().color = textColor;
    }
}
