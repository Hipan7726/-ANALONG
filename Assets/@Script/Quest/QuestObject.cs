using UnityEngine;

/// <summary>
/// NPC ������Ʈ�� �پ� ����Ʈ ���� �Ǵ� ����Ʈ ���� Ȯ�� ������ ����ϴ� ��ũ��Ʈ
/// </summary>
public class QuestObject : MonoBehaviour
{
    public string questId;         // �� NPC�� ���õ� ����Ʈ�� ID
    public bool isStartQuest;      // true�̸� ����Ʈ�� �����ϴ� NPC, false�̸� ����Ʈ ���¸� Ȯ���ϴ� NPC
    public string npcName;         // �� NPC�� �̸� (��ȭâ � ǥ�õ�)

    private bool isInteracting = false;  // ��ȣ�ۿ� �ߺ� ���� �÷���

    /// <summary>
    /// �÷��̾ E Ű�� NPC�� ��ȣ�ۿ��� �� ȣ��Ǵ� �Լ�
    /// </summary>
    public void Interact()
    {
        Debug.Log($"[Interact] EŰ ���� - NPC: {npcName}, isStartQuest: {isStartQuest}, QuestId: {questId}");

        DialogueManager.Instance.StartDialogue(npcName, () =>
        {
            Debug.Log("[Interact] ��ȭ ������ �� �ݹ� �����");
            QuestManager.Instance.CheckQuestProgress(npcName);
        });

        if (isStartQuest)
        {
            QuestManager.Instance.StartQuest(questId);
        }
    }
}

