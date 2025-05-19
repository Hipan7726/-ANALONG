using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance; // �̱��� �ν��Ͻ�

    public GameObject navigationDotPrefab; // ��� ǥ�ø� ���� ��Ʈ ������
    public float spacing = 1.5f; // ��Ʈ ����
    public int maxDots = 50;     // �ִ� ��Ʈ ��

    private List<GameObject> activeDots = new(); // Ȱ��ȭ�� ��Ʈ ���

    private void Awake()
    {
        if (Instance == null) Instance = this; // �̱��� ����
    }

    // ���� ����(from)�� ��ǥ ����(to) ���̿� ��θ� �����ϰ� ��Ʈ�� ǥ��
    public void CreatePath(Vector3 from, Vector3 to)
    {
        ClearPath(); // ���� ��� ����

        NavMeshPath path = new NavMeshPath(); // ��θ� ������ ��ü

        // NavMesh�� ����� ��� ���
        if (NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path))
        {
            Vector3 previousCorner = from;

            // ���� ����� �� �ڳʿ� ���� ó��
            for (int i = 1; i < path.corners.Length; i++)
            {
                Vector3 currentCorner = path.corners[i];
                float segmentDistance = Vector3.Distance(previousCorner, currentCorner); // �� �� �� �Ÿ�
                Vector3 segmentDirection = (currentCorner - previousCorner).normalized; // ���� ����

                // �� �� ���� ���ݿ� ���� ��Ʈ ���� ���
                int dotCount = Mathf.FloorToInt(segmentDistance / spacing);

                // �� �������� ��Ʈ ����
                for (int j = 1; j <= dotCount; j++)
                {
                    Vector3 pos = previousCorner + segmentDirection * spacing * j; // ��Ʈ ��ġ ���
                    GameObject dot = Instantiate(navigationDotPrefab, pos + Vector3.up * 0.1f, Quaternion.identity); // ��Ʈ ����
                    activeDots.Add(dot); // ������ ��Ʈ�� ��Ͽ� �߰�

                    // ��Ʈ ���� �ִ�ġ�� �����ϸ� �� �̻� �������� ����
                    if (activeDots.Count >= maxDots)
                        return;
                }

                previousCorner = currentCorner; // ���� �ڳʸ� ���� �ڳʷ� ������Ʈ
            }
        }

        // ���� �ð� �� ��Ʈ ����
        StartCoroutine(RemoveDotsAfterSeconds(3f));
    }

    // ��θ� �����ϴ� �Լ�
    public void ClearPath()
    {
        // Ȱ��ȭ�� ��Ʈ���� ��� ����
        foreach (var dot in activeDots)
        {
            Destroy(dot);
        }
        activeDots.Clear(); // ��Ʈ ��� ����
    }

    // ���� �ð��� ������ ��Ʈ�� �ڵ����� �����ϴ� �ڷ�ƾ
    private IEnumerator RemoveDotsAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds); // ������ �ð� ���� ���
        ClearPath(); // �ð� ������ ��� ����
    }
}
