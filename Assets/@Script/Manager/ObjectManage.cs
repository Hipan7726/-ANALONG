using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManage : MonoBehaviour
{
    [SerializeField]
    private GameObject Object; // ������ ��� ��ü
    private bool check = false; // ���� ���¸� �����ϴ� ����

    // ���� ȿ���� ����ϴ� �Լ�
    public void SetEmphasis()
    {
        if (!check)
        {
            // ���� ����
            StartCoroutine("Emphasis", Object);
            check = true;
        }
        else
        {
            // ���� ����
            StopCoroutine("Emphasis");
            check = false;
        }
    }

    // ���� ȿ���� �ִ� �ڷ�ƾ
    private IEnumerator Emphasis(GameObject gameObject)
    {
        float increase = 0.1f; // ũ�� ������ ������

        // ���� ���� (ũ�� ������ �ݺ�)
        while (true)
        {
            // ��ü�� �ּ� ũ�� 0.5f �̻��� �� ������ ũ�� ���
            while (gameObject.GetComponent<Transform>().localScale.x > 0.5f)
            {
                // ũ�⸦ ���
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - increase
                                                                , gameObject.transform.localScale.y - increase
                                                                , gameObject.transform.localScale.z - increase);
                yield return new WaitForSeconds(0.05f); // ũ�Ⱑ �ٲ�� �ӵ�
            }
            yield return new WaitForSeconds(0.05f); // �߰��� ���ߴ� ��

            // ��ü�� ���� ũ��(1f)�� ���ƿ� ������ ũ�� ����
            while (gameObject.GetComponent<Transform>().localScale.x < 1f)
            {
                // ũ�⸦ ����
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x + increase
                                                                , gameObject.transform.localScale.y + increase
                                                                , gameObject.transform.localScale.z + increase);
                yield return new WaitForSeconds(0.05f); // ũ�Ⱑ �ٲ�� �ӵ�
            }
            yield return new WaitForSeconds(0.05f); // �߰��� ���ߴ� ��
        }
    }
}
