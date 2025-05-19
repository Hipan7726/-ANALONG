using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

/// <summary>
/// 퀘스트 데이터를 로드하고, 퀘스트 상태를 관리하며,
/// 퀘스트의 시작/완료/진행 확인 등을 처리하는 매니저 클래스.
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public TextMeshProUGUI questTitleText;  // 퀘스트 제목을 표시할 UI 텍스트
    public TextMeshProUGUI moneyText;       // 현재 금액을 표시할 UI 텍스트
    public TextMeshProUGUI gpText;          // 현재 경험치를 표시할 UI 텍스트

    public int currentMoney = 0;  // 현재 금액
    public int currentGP = 0;     // 현재 경험치

    private Dictionary<string, Quest> questDB = new();  // 퀘스트 데이터베이스
    private Dictionary<string, Quest> activeQuests = new();  // 진행 중인 퀘스트
    private HashSet<string> completedQuests = new();  // 완료된 퀘스트

    // QuestManager 인스턴스를 초기화
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // 게임 시작 시 퀘스트 데이터를 로드하고, 초기 퀘스트를 시작
    private void Start()
    {
        LoadQuestData();

        Cursor.lockState = CursorLockMode.Locked;  // 커서를 잠금
        Cursor.visible = false;  // 커서 숨기기

        // q001 퀘스트가 이미 시작되었거나 완료되었는지 확인
        if (!IsQuestActiveOrComplete("q001"))
        {
            StartQuest("q001");  // q001 퀘스트 시작
        }
    }


    // 퀘스트 데이터 파일(quests.json)을 로드하여 퀘스트 정보를 questDB에 저장
    private void LoadQuestData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "quests.json");

        if (!File.Exists(path))
        {
            Debug.LogError("퀘스트 파일이 존재하지 않음: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        QuestList questList = JsonUtility.FromJson<QuestList>("{\"quests\":" + json + "}");  // JSON 파싱

        // 퀘스트 데이터를 questDB에 저장
        foreach (var quest in questList.quests)
        {
            questDB[quest.id] = quest;
        }

        Debug.Log($"총 {questDB.Count}개의 퀘스트 로드됨");
    }

    // 퀘스트를 시작하는 함수
    public void StartQuest(string questId)
    {
        // 이미 시작되었거나 완료된 퀘스트라면 시작하지 않음
        if (activeQuests.ContainsKey(questId) || completedQuests.Contains(questId))
        {
            Debug.Log($"[QuestManager] 이미 시작했거나 완료된 퀘스트: {questId}");
            return;
        }

        // 퀘스트가 존재하면 시작
        if (questDB.TryGetValue(questId, out Quest quest))
        {
            activeQuests[questId] = quest;
            Debug.Log($"[QuestManager] 퀘스트 시작: {quest.title}");

            questTitleText.text = $"{quest.title}";  // UI에 퀘스트 제목 표시
        }
        else
        {
            Debug.LogWarning($"[QuestManager] 해당 ID의 퀘스트가 없음: {questId}");
        }
    }

    // 퀘스트를 완료하는 함수
    public void CompleteQuest(string questId)
    {
        if (!activeQuests.ContainsKey(questId)) return;

        Quest quest = activeQuests[questId];
        activeQuests.Remove(questId);  // 진행 중인 퀘스트에서 제거
        completedQuests.Add(questId);  // 완료된 퀘스트에 추가

        Debug.Log($"[QuestManager] 퀘스트 완료: {quest.title} / 보상: 골드 {quest.rewards.gold}, 경험치 {quest.rewards.gp}");

        // 보상 지급
        currentMoney += quest.rewards.gold;
        currentGP += quest.rewards.gp;
        GameManager.Instance.LoadedGP = currentGP;

        // UI 업데이트
        moneyText.text = $"{currentMoney}";
        gpText.text = $"{currentGP}";

        // 다음 퀘스트가 있으면 시작
        if (!string.IsNullOrEmpty(quest.nextQuestId))
        {
            StartQuest(quest.nextQuestId);
            GameManager.Instance.SaveGame();
        }
        else
        {
            questTitleText.text = "퀘스트 없음";  // 퀘스트가 없으면 "퀘스트 없음" 표시
        }
    }

    // 특정 NPC와 대화하여 퀘스트 진행 여부를 확인하고, 진행 조건을 만족하면 완료 처리
    public void CheckQuestProgress(string npcName)
    {
        foreach (var quest in activeQuests.Values)
        {
            if (quest.triggerType == "TalkToNPC" && quest.target == npcName)
            {
                Debug.Log($"[QuestManager] 퀘스트 '{quest.title}' 진행 완료 조건 만족 (NPC: {npcName})");
                CompleteQuest(quest.id);
                return;
            }
        }

        Debug.Log($"[QuestManager] 현재 {npcName}와 관련된 진행 중인 퀘스트 없음");
    }

    // 퀘스트가 진행 중이거나 완료된 상태인지 확인
    public bool IsQuestActiveOrComplete(string questId)
    {
        return activeQuests.ContainsKey(questId) || completedQuests.Contains(questId);
    }

    public bool IsQuestCompleted(string npcName)
    {
        return completedQuests.Contains(npcName);
    }

    // NPC와 관련된 퀘스트가 완료되었는지 확인
    public bool IsQuestCompletedByNPC(string npcName)
    {
        foreach (var questId in completedQuests)
        {
            if (questDB.TryGetValue(questId, out Quest quest))
            {
                if (quest.target == npcName)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 현재 진행 중인 퀘스트를 반환
    public Quest GetCurrentQuest()
    {
        foreach (var quest in activeQuests.Values)
        {
            return quest;
        }
        return null;
    }

    // GameManager 연동용
    public List<string> GetActiveQuestIds() => new(activeQuests.Keys);
    public List<string> GetCompletedQuestIds() => new(completedQuests);

    // GameManager와 연동하여 진행 중인 퀘스트와 완료된 퀘스트를 로드
    public void LoadFromGameData(GameData data, GachaData gata)
    {
        activeQuests.Clear();
        completedQuests.Clear();

        foreach (string id in data.ActiveQuestIds)
        {
            if (questDB.TryGetValue(id, out Quest quest))
            {
                activeQuests[id] = quest;
            }
        }

        foreach (string id in data.CompletedQuestIds)
        {
            completedQuests.Add(id);
        }

        currentMoney = data.Money;
        currentGP = gata.Gp;

        moneyText.text = $"{currentMoney}";
        gpText.text = $"{currentGP}";

        if (activeQuests.Count > 0)
        {
            questTitleText.text = $"{GetCurrentQuest()?.title}";
        }
        else
        {
            questTitleText.text = "퀘스트 없음";
        }
    }
    // 퀘스트를 초기화하는 함수
    public void ResetQuests()
    {
        activeQuests.Clear();
        completedQuests.Clear();
        currentMoney = 0;
        currentGP = 0;

        questTitleText.text = "퀘스트 없음";
        moneyText.text = "0";
        gpText.text = "0";

        StartQuest("q001");
    }
}
