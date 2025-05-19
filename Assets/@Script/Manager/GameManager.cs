// ���� �����͸� �����ϴ� Ŭ����
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class GameData
{
    public List<string> ActiveQuestIds = new();  // Ȱ��ȭ�� ����Ʈ ID ���
    public List<string> CompletedQuestIds = new(); // �Ϸ�� ����Ʈ ID ���
    public List<string> UnlockedCharacterIds = new(); // ��� ������ ĳ���� ID ���
    public int Money; // ���� �ݾ�
    public Vector3 PlayerPosition; // �÷��̾� ��ġ
    public Vector3 PlayerRotation; // �÷��̾� ȸ�� ��
    public Vector3 CameraPosition; // ī�޶� ��ġ
    public Vector3 CameraRotation; // ī�޶� ȸ�� ��
}

// ��í �����͸� �����ϴ� Ŭ����
[System.Serializable]
public class GachaData
{
    public int Gp; // Gacha ����Ʈ (GP)
    public int DrawCount; // ��í ��ο� Ƚ��
    public bool SLock; // S��ũ ��� ����
}

// ������ �ֿ� ������ ó���ϴ� �Ŵ��� Ŭ����
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // �̱��� �ν��Ͻ�

    // UI ������Ʈ��
    public GameObject Menu;
    public GameObject MusicMenu;
    public Button SaveButton;
    public Button MusicButton;
    public Button GameButton;
    public Button BackButtonA;
    public Button BackButtonB;

    public int LoadedGP { get; set; } // �ҷ��� Gacha ����Ʈ
    public int LoadedDrawCount { get; set; } // �ҷ��� ��ο� Ƚ��
    public bool LoadedSLock { get; private set; } // �ܺ� ���� ���� SLock ��

    public List<string> UnlockedCharacterIds = new(); // ��� ������ ĳ���� ID ���

    public bool SetPlayer; // �÷��̾� ���� ���� ����
    private string _savePath; // ���� ���
    private string _gachaSavePath; // ��í ���� ���

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            _savePath = Path.Combine(Application.persistentDataPath, "save.json");
            _gachaSavePath = Path.Combine(Application.persistentDataPath, "gacha_save.json");
            DontDestroyOnLoad(gameObject);
            //DontDestroyOnLoad(Menu); // Menu�� �� �̵� �� �ı����� �ʵ���
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� ������ �ڱ� �ڽ� �ı�
            return;
        }
    }

    private void Start()
    {
        SetPlayer = true;
        SoundManager.Instance.BgmSource.Stop(); // ������� ����
        SoundManager.Instance.PlayStartGame(); // ���� ������ ���

        // ��ư Ŭ�� �̺�Ʈ ���
        SaveButton.onClick.AddListener(SaveGame);
        MusicButton.onClick.AddListener(MusicTrue);
        GameButton.onClick.AddListener(GameTermination);
        BackButtonA.onClick.AddListener(MeunFalse);
        BackButtonB.onClick.AddListener(MusicFalse);
    }

    private void Update()
    {
        // �÷��̾� ���¿� ���� �ִϸ��̼��� ����
        if (SetPlayer == false)
        {
            PlayerController.Instance._animator.SetFloat(Define.Speed, 0);
        }

        // Ű �Է¿� ���� ��� ����
        if (Input.GetKeyDown(KeyCode.F5)) SaveGame(); // F5: ���� ����
        if (Input.GetKeyDown(KeyCode.F9)) LoadGame(); // F9: ���� �ҷ�����
        if (Input.GetKeyDown(KeyCode.F1)) ResetGame(); // F1: ���� ����
        if (Input.GetKeyDown(KeyCode.Escape)) MENU(); // Esc: �޴� ����
    }

    void MENU()
    {
        SetPlayer = false;
        SoundManager.Instance.PlayButton();

        Cursor.lockState = CursorLockMode.None; // Ŀ�� ��� ����
        Cursor.visible = true; // Ŀ�� ǥ��

        Menu.SetActive(true); // �޴� Ȱ��ȭ
    }

    void MusicTrue()
    {
        SoundManager.Instance.PlayButton();
        MusicMenu.SetActive(true); // ���� �޴� Ȱ��ȭ
    }

    void MeunFalse()
    {
        SetPlayer = true;
        SoundManager.Instance.PlayButton();

        Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ���
        Cursor.visible = false; // Ŀ�� �����

        Menu.SetActive(false); // �޴� ��Ȱ��ȭ
    }

    void MusicFalse()
    {
        SoundManager.Instance.PlayButton();
        MusicMenu.SetActive(false); // ���� �޴� ��Ȱ��ȭ
    }

    void GameTermination()
    {
        SoundManager.Instance.PlayButton();
        Application.Quit(); // ���� ����
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ��� ���� ���� ó��
        #endif
    }

    // ���� ����
    public void SaveGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "DrawScrene")
        {
            SaveGachaDataOnly(); // ��í �����͸� ����
            return;
        }

        // ���� ������ ��ü ���� �� ä���
        GameData data = new GameData
        {
            ActiveQuestIds = QuestManager.Instance.GetActiveQuestIds(),
            CompletedQuestIds = QuestManager.Instance.GetCompletedQuestIds(),
            Money = QuestManager.Instance.currentMoney,
            UnlockedCharacterIds = new List<string>(UnlockedCharacterIds)
        };

        // �÷��̾� ������ ����
        if (PlayerController.Instance != null)
        {
            data.PlayerPosition = PlayerController.Instance.GetPlayerPosition();
            data.PlayerRotation = PlayerController.Instance.GetPlayerRotation();
            data.CameraPosition = PlayerController.Instance.GetCameraPosition();
            data.CameraRotation = PlayerController.Instance.GetCameraRotation();
        }

        // ������ ���Ϸ� ����
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"[GameManager] ���� ���� �Ϸ�: {_savePath}");

        SaveGachaDataOnly(); // ��í �����͵� ����
    }

    // Gacha �����͸� ����
    private void SaveGachaDataOnly()
    {
        GachaData gachaData = new GachaData
        {
            Gp = QuestManager.Instance.currentGP,
            DrawCount = LoadedDrawCount,
            SLock = LoadedSLock
        };

        string gachaJson = JsonUtility.ToJson(gachaData, true);
        File.WriteAllText(_gachaSavePath, gachaJson);
        Debug.Log($"[GameManager] Gacha ������ ���� �Ϸ�: {_gachaSavePath}");
    }

    // ���� �ҷ�����
    public void LoadGame()
    {
        GachaData gata = null;

        // ��í ������ �ҷ�����
        if (File.Exists(_gachaSavePath))
        {
            string gachaJson = File.ReadAllText(_gachaSavePath);
            gata = JsonUtility.FromJson<GachaData>(gachaJson);

            LoadedGP = gata.Gp;
            LoadedDrawCount = gata.DrawCount;
            LoadedSLock = gata.SLock;
            QuestManager.Instance.currentGP = LoadedGP;

            Debug.Log("[GameManager] Gacha ������ �ҷ����� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("[GameManager] Gacha ���� ���� ����");
            gata = new GachaData();
        }

        // ���� ���� ������ �ҷ�����
        if (!File.Exists(_savePath))
        {
            Debug.LogWarning("[GameManager] ���� ���� ����");
            return;
        }

        string json = File.ReadAllText(_savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        // QuestManager�� �÷��̾� ������ �ҷ�����
        QuestManager.Instance.LoadFromGameData(data, gata);

        if (SceneManager.GetActiveScene().name != "DrawScrene" && PlayerController.Instance != null)
        {
            PlayerController.Instance.transform.position = data.PlayerPosition;
            PlayerController.Instance._character.eulerAngles = data.PlayerRotation;
            PlayerController.Instance._camera.transform.localPosition = data.CameraPosition;
            PlayerController.Instance._camAxis.rotation = Quaternion.Euler(data.CameraRotation);
        }

        UnlockedCharacterIds = new List<string>(data.UnlockedCharacterIds);
        Debug.Log("[GameManager] ���� �ҷ����� �Ϸ�");
    }

    // ���� ����
    public void ResetGame()
    {
        if (File.Exists(_savePath)) File.Delete(_savePath);
        if (File.Exists(_gachaSavePath)) File.Delete(_gachaSavePath);

        QuestManager.Instance.ResetQuests(); // ����Ʈ ����
        UnlockedCharacterIds.Clear(); // ��� ������ ĳ���� ��� �ʱ�ȭ
        Debug.Log("[GameManager] ���� ���� �Ϸ�");
    }

    // GP ���
    public void UseGP(int amount)
    {
        LoadedGP -= amount;
        QuestManager.Instance.currentGP = LoadedGP;
    }

    // SLock �� ���� �� ����
    public void SetSLock(bool value)
    {
        LoadedSLock = value;
        SaveGachaDataOnly(); // ���� ��� ����
    }

    // ĳ���� ��� ����
    public void UnlockCharacter(string characterId)
    {
        if (!UnlockedCharacterIds.Contains(characterId))
        {
            UnlockedCharacterIds.Add(characterId);
            SaveGame(); // ���� ����
        }
    }

    // ĳ���� ��� ���� ���� Ȯ��
    public bool IsCharacterUnlocked(string characterId)
    {
        return UnlockedCharacterIds.Contains(characterId);
    }
}
