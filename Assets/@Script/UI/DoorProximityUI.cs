using System.Collections.Generic;
using UnityEngine;

public class DoorProximityUI : MonoBehaviour
{
    public List<Transform> doors;           // �� ����Ʈ
    public Transform buttonUI;              // ��ư UI (�ϳ��� ���)
    public float triggerDistance = 1.5f;      // ��ư�� Ŀ�� �Ÿ�
    public Vector3 enlargedScale = new Vector3(1.2f, 1.2f, 1.2f);
    public float scaleSpeed = 5f;
    public GameObject Line;

    private Transform player;
    private bool isNearAnyDoor = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || buttonUI == null || doors.Count == 0) return;

        isNearAnyDoor = false;
        Line.SetActive(false);

        foreach (var door in doors)
        {
            if (Vector3.Distance(player.position, door.position) <= triggerDistance)
            {
                isNearAnyDoor = true;
                Line.SetActive(true);
                break;
            }
        }

        Vector3 targetScale = isNearAnyDoor ? enlargedScale : Vector3.one;
        buttonUI.localScale = Vector3.Lerp(buttonUI.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }
}
