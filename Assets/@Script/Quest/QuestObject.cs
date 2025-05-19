using UnityEngine;

/// <summary>
/// NPC 오브젝트에 붙어 퀘스트 시작 또는 퀘스트 상태 확인 역할을 담당하는 스크립트
/// </summary>
public class QuestObject : MonoBehaviour
{
    public string questId;         // 이 NPC와 관련된 퀘스트의 ID
    public bool isStartQuest;      // true이면 퀘스트를 시작하는 NPC, false이면 퀘스트 상태를 확인하는 NPC
    public string npcName;         // 이 NPC의 이름 (대화창 등에 표시됨)

    private bool isInteracting = false;  // 상호작용 중복 방지 플래그

    /// <summary>
    /// 플레이어가 E 키로 NPC와 상호작용할 때 호출되는 함수
    /// </summary>
    public void Interact()
    {
        Debug.Log($"[Interact] E키 눌림 - NPC: {npcName}, isStartQuest: {isStartQuest}, QuestId: {questId}");

        DialogueManager.Instance.StartDialogue(npcName, () =>
        {
            Debug.Log("[Interact] 대화 끝났을 때 콜백 실행됨");
            QuestManager.Instance.CheckQuestProgress(npcName);
        });

        if (isStartQuest)
        {
            QuestManager.Instance.StartQuest(questId);
        }
    }
}

