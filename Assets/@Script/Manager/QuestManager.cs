using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

/// <summary>
/// ����Ʈ �����͸� �ε��ϰ�, ����Ʈ ���¸� �����ϸ�,
/// ����Ʈ�� ����/�Ϸ�/���� Ȯ�� ���� ó���ϴ� �Ŵ��� Ŭ����.
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public TextMeshProUGUI questTitleText;  // ����Ʈ ������ ǥ���� UI �ؽ�Ʈ
    public TextMeshProUGUI moneyText;       // ���� �ݾ��� ǥ���� UI �ؽ�Ʈ
    public TextMeshProUGUI gpText;          // ���� ����ġ�� ǥ���� UI �ؽ�Ʈ

    public int currentMoney = 0;  // ���� �ݾ�
    public int currentGP = 0;     // ���� ����ġ

    private Dictionary<string, Quest> questDB = new();  // ����Ʈ �����ͺ��̽�
    private Dictionary<string, Quest> activeQuests = new();  // ���� ���� ����Ʈ
    private HashSet<string> completedQuests = new();  // �Ϸ�� ����Ʈ

    // QuestManager �ν��Ͻ��� �ʱ�ȭ
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // ���� ���� �� ����Ʈ �����͸� �ε��ϰ�, �ʱ� ����Ʈ�� ����
    private void Start()
    {
        LoadQuestData();

        Cursor.lockState = CursorLockMode.Locked;  // Ŀ���� ���
        Cursor.visible = false;  // Ŀ�� �����

        // q001 ����Ʈ�� �̹� ���۵Ǿ��ų� �Ϸ�Ǿ����� Ȯ��
        if (!IsQuestActiveOrComplete("q001"))
        {
            StartQuest("q001");  // q001 ����Ʈ ����
        }
    }


    // ����Ʈ ������ ����(quests.json)�� �ε��Ͽ� ����Ʈ ������ questDB�� ����
    private void LoadQuestData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "quests.json");

        if (!File.Exists(path))
        {
            Debug.LogError("����Ʈ ������ �������� ����: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        QuestList questList = JsonUtility.FromJson<QuestList>("{\"quests\":" + json + "}");  // JSON �Ľ�

        // ����Ʈ �����͸� questDB�� ����
        foreach (var quest in questList.quests)
        {
            questDB[quest.id] = quest;
        }

        Debug.Log($"�� {questDB.Count}���� ����Ʈ �ε��");
    }

    // ����Ʈ�� �����ϴ� �Լ�
    public void StartQuest(string questId)
    {
        // �̹� ���۵Ǿ��ų� �Ϸ�� ����Ʈ��� �������� ����
        if (activeQuests.ContainsKey(questId) || completedQuests.Contains(questId))
        {
            Debug.Log($"[QuestManager] �̹� �����߰ų� �Ϸ�� ����Ʈ: {questId}");
            return;
        }

        // ����Ʈ�� �����ϸ� ����
        if (questDB.TryGetValue(questId, out Quest quest))
        {
            activeQuests[questId] = quest;
            Debug.Log($"[QuestManager] ����Ʈ ����: {quest.title}");

            questTitleText.text = $"{quest.title}";  // UI�� ����Ʈ ���� ǥ��
        }
        else
        {
            Debug.LogWarning($"[QuestManager] �ش� ID�� ����Ʈ�� ����: {questId}");
        }
    }

    // ����Ʈ�� �Ϸ��ϴ� �Լ�
    public void CompleteQuest(string questId)
    {
        if (!activeQuests.ContainsKey(questId)) return;

        Quest quest = activeQuests[questId];
        activeQuests.Remove(questId);  // ���� ���� ����Ʈ���� ����
        completedQuests.Add(questId);  // �Ϸ�� ����Ʈ�� �߰�

        Debug.Log($"[QuestManager] ����Ʈ �Ϸ�: {quest.title} / ����: ��� {quest.rewards.gold}, ����ġ {quest.rewards.gp}");

        // ���� ����
        currentMoney += quest.rewards.gold;
        currentGP += quest.rewards.gp;
        GameManager.Instance.LoadedGP = currentGP;

        // UI ������Ʈ
        moneyText.text = $"{currentMoney}";
        gpText.text = $"{currentGP}";

        // ���� ����Ʈ�� ������ ����
        if (!string.IsNullOrEmpty(quest.nextQuestId))
        {
            StartQuest(quest.nextQuestId);
            GameManager.Instance.SaveGame();
        }
        else
        {
            questTitleText.text = "����Ʈ ����";  // ����Ʈ�� ������ "����Ʈ ����" ǥ��
        }
    }

    // Ư�� NPC�� ��ȭ�Ͽ� ����Ʈ ���� ���θ� Ȯ���ϰ�, ���� ������ �����ϸ� �Ϸ� ó��
    public void CheckQuestProgress(string npcName)
    {
        foreach (var quest in activeQuests.Values)
        {
            if (quest.triggerType == "TalkToNPC" && quest.target == npcName)
            {
                Debug.Log($"[QuestManager] ����Ʈ '{quest.title}' ���� �Ϸ� ���� ���� (NPC: {npcName})");
                CompleteQuest(quest.id);
                return;
            }
        }

        Debug.Log($"[QuestManager] ���� {npcName}�� ���õ� ���� ���� ����Ʈ ����");
    }

    // ����Ʈ�� ���� ���̰ų� �Ϸ�� �������� Ȯ��
    public bool IsQuestActiveOrComplete(string questId)
    {
        return activeQuests.ContainsKey(questId) || completedQuests.Contains(questId);
    }

    public bool IsQuestCompleted(string npcName)
    {
        return completedQuests.Contains(npcName);
    }

    // NPC�� ���õ� ����Ʈ�� �Ϸ�Ǿ����� Ȯ��
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

    // ���� ���� ���� ����Ʈ�� ��ȯ
    public Quest GetCurrentQuest()
    {
        foreach (var quest in activeQuests.Values)
        {
            return quest;
        }
        return null;
    }

    // GameManager ������
    public List<string> GetActiveQuestIds() => new(activeQuests.Keys);
    public List<string> GetCompletedQuestIds() => new(completedQuests);

    // GameManager�� �����Ͽ� ���� ���� ����Ʈ�� �Ϸ�� ����Ʈ�� �ε�
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
            questTitleText.text = "����Ʈ ����";
        }
    }
    // ����Ʈ�� �ʱ�ȭ�ϴ� �Լ�
    public void ResetQuests()
    {
        activeQuests.Clear();
        completedQuests.Clear();
        currentMoney = 0;
        currentGP = 0;

        questTitleText.text = "����Ʈ ����";
        moneyText.text = "0";
        gpText.text = "0";

        StartQuest("q001");
    }
}
