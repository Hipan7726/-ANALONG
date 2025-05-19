using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����Ʈ ������ ��� Ŭ����
/// </summary>
[System.Serializable]
public class Quest
{
    public string id;               // ����Ʈ ���� ID
    public string title;            // ����Ʈ ����
    public string description;      // ����Ʈ ����
    public string triggerType;      // ����Ʈ �Ϸ� ���� Ÿ�� (��: "TalkToNPC", "CollectItem" ��)
    public string target;           // ����Ʈ ��ǥ ��� (��: Ư�� NPC �̸�, ������ �̸� ��)
    public string nextQuestId;      // �� ����Ʈ �Ϸ� �� �ڵ����� ���۵� ���� ����Ʈ ID
    public QuestReward rewards;     // ����Ʈ �Ϸ� �� ���޵Ǵ� ����

    public Vector3Serializable targetPosition;  // �߰�
}

    /// <summary>
    /// ����Ʈ ���� ������ ��� Ŭ����
    /// </summary>
    [System.Serializable]
public class QuestReward
{
    public int gold;                // ȹ�� ���
    public int gp;                  // ȹ�� ����ġ �Ǵ� ���� ����Ʈ
}

/// <summary>
/// JSON �Ľ��� ���� ����Ʈ ����Ʈ ���� Ŭ����
/// </summary>
[System.Serializable]
public class QuestList
{
    public List<Quest> quests;      // ��ü ����Ʈ ���
}

[System.Serializable]
public class Vector3Serializable
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3() => new(x, y, z);
}

