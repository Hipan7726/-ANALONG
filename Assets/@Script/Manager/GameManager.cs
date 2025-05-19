// 게임 데이터를 저장하는 클래스
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class GameData
{
    public List<string> ActiveQuestIds = new();  // 활성화된 퀘스트 ID 목록
    public List<string> CompletedQuestIds = new(); // 완료된 퀘스트 ID 목록
    public List<string> UnlockedCharacterIds = new(); // 잠금 해제된 캐릭터 ID 목록
    public int Money; // 현재 금액
    public Vector3 PlayerPosition; // 플레이어 위치
    public Vector3 PlayerRotation; // 플레이어 회전 값
    public Vector3 CameraPosition; // 카메라 위치
    public Vector3 CameraRotation; // 카메라 회전 값
}

// 가챠 데이터를 저장하는 클래스
[System.Serializable]
public class GachaData
{
    public int Gp; // Gacha 포인트 (GP)
    public int DrawCount; // 가챠 드로우 횟수
    public bool SLock; // S랭크 잠금 여부
}

// 게임의 주요 로직을 처리하는 매니저 클래스
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤 인스턴스

    // UI 오브젝트들
    public GameObject Menu;
    public GameObject MusicMenu;
    public Button SaveButton;
    public Button MusicButton;
    public Button GameButton;
    public Button BackButtonA;
    public Button BackButtonB;

    public int LoadedGP { get; set; } // 불러온 Gacha 포인트
    public int LoadedDrawCount { get; set; } // 불러온 드로우 횟수
    public bool LoadedSLock { get; private set; } // 외부 접근 허용된 SLock 값

    public List<string> UnlockedCharacterIds = new(); // 잠금 해제된 캐릭터 ID 목록

    public bool SetPlayer; // 플레이어 상태 설정 여부
    private string _savePath; // 저장 경로
    private string _gachaSavePath; // 가챠 저장 경로

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            _savePath = Path.Combine(Application.persistentDataPath, "save.json");
            _gachaSavePath = Path.Combine(Application.persistentDataPath, "gacha_save.json");
            DontDestroyOnLoad(gameObject);
            //DontDestroyOnLoad(Menu); // Menu도 씬 이동 시 파괴되지 않도록
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 자기 자신 파괴
            return;
        }
    }

    private void Start()
    {
        SetPlayer = true;
        SoundManager.Instance.BgmSource.Stop(); // 배경음악 정지
        SoundManager.Instance.PlayStartGame(); // 게임 시작음 재생

        // 버튼 클릭 이벤트 등록
        SaveButton.onClick.AddListener(SaveGame);
        MusicButton.onClick.AddListener(MusicTrue);
        GameButton.onClick.AddListener(GameTermination);
        BackButtonA.onClick.AddListener(MeunFalse);
        BackButtonB.onClick.AddListener(MusicFalse);
    }

    private void Update()
    {
        // 플레이어 상태에 따라 애니메이션을 설정
        if (SetPlayer == false)
        {
            PlayerController.Instance._animator.SetFloat(Define.Speed, 0);
        }

        // 키 입력에 따른 기능 수행
        if (Input.GetKeyDown(KeyCode.F5)) SaveGame(); // F5: 게임 저장
        if (Input.GetKeyDown(KeyCode.F9)) LoadGame(); // F9: 게임 불러오기
        if (Input.GetKeyDown(KeyCode.F1)) ResetGame(); // F1: 게임 리셋
        if (Input.GetKeyDown(KeyCode.Escape)) MENU(); // Esc: 메뉴 열기
    }

    void MENU()
    {
        SetPlayer = false;
        SoundManager.Instance.PlayButton();

        Cursor.lockState = CursorLockMode.None; // 커서 잠금 해제
        Cursor.visible = true; // 커서 표시

        Menu.SetActive(true); // 메뉴 활성화
    }

    void MusicTrue()
    {
        SoundManager.Instance.PlayButton();
        MusicMenu.SetActive(true); // 음악 메뉴 활성화
    }

    void MeunFalse()
    {
        SetPlayer = true;
        SoundManager.Instance.PlayButton();

        Cursor.lockState = CursorLockMode.Locked; // 커서 잠금
        Cursor.visible = false; // 커서 숨기기

        Menu.SetActive(false); // 메뉴 비활성화
    }

    void MusicFalse()
    {
        SoundManager.Instance.PlayButton();
        MusicMenu.SetActive(false); // 음악 메뉴 비활성화
    }

    void GameTermination()
    {
        SoundManager.Instance.PlayButton();
        Application.Quit(); // 게임 종료
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 게임 종료 처리
        #endif
    }

    // 게임 저장
    public void SaveGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "DrawScrene")
        {
            SaveGachaDataOnly(); // 가챠 데이터만 저장
            return;
        }

        // 게임 데이터 객체 생성 및 채우기
        GameData data = new GameData
        {
            ActiveQuestIds = QuestManager.Instance.GetActiveQuestIds(),
            CompletedQuestIds = QuestManager.Instance.GetCompletedQuestIds(),
            Money = QuestManager.Instance.currentMoney,
            UnlockedCharacterIds = new List<string>(UnlockedCharacterIds)
        };

        // 플레이어 데이터 저장
        if (PlayerController.Instance != null)
        {
            data.PlayerPosition = PlayerController.Instance.GetPlayerPosition();
            data.PlayerRotation = PlayerController.Instance.GetPlayerRotation();
            data.CameraPosition = PlayerController.Instance.GetCameraPosition();
            data.CameraRotation = PlayerController.Instance.GetCameraRotation();
        }

        // 데이터 파일로 저장
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"[GameManager] 게임 저장 완료: {_savePath}");

        SaveGachaDataOnly(); // 가챠 데이터도 저장
    }

    // Gacha 데이터만 저장
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
        Debug.Log($"[GameManager] Gacha 데이터 저장 완료: {_gachaSavePath}");
    }

    // 게임 불러오기
    public void LoadGame()
    {
        GachaData gata = null;

        // 가챠 데이터 불러오기
        if (File.Exists(_gachaSavePath))
        {
            string gachaJson = File.ReadAllText(_gachaSavePath);
            gata = JsonUtility.FromJson<GachaData>(gachaJson);

            LoadedGP = gata.Gp;
            LoadedDrawCount = gata.DrawCount;
            LoadedSLock = gata.SLock;
            QuestManager.Instance.currentGP = LoadedGP;

            Debug.Log("[GameManager] Gacha 데이터 불러오기 완료");
        }
        else
        {
            Debug.LogWarning("[GameManager] Gacha 저장 파일 없음");
            gata = new GachaData();
        }

        // 게임 저장 데이터 불러오기
        if (!File.Exists(_savePath))
        {
            Debug.LogWarning("[GameManager] 저장 파일 없음");
            return;
        }

        string json = File.ReadAllText(_savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        // QuestManager와 플레이어 데이터 불러오기
        QuestManager.Instance.LoadFromGameData(data, gata);

        if (SceneManager.GetActiveScene().name != "DrawScrene" && PlayerController.Instance != null)
        {
            PlayerController.Instance.transform.position = data.PlayerPosition;
            PlayerController.Instance._character.eulerAngles = data.PlayerRotation;
            PlayerController.Instance._camera.transform.localPosition = data.CameraPosition;
            PlayerController.Instance._camAxis.rotation = Quaternion.Euler(data.CameraRotation);
        }

        UnlockedCharacterIds = new List<string>(data.UnlockedCharacterIds);
        Debug.Log("[GameManager] 게임 불러오기 완료");
    }

    // 게임 리셋
    public void ResetGame()
    {
        if (File.Exists(_savePath)) File.Delete(_savePath);
        if (File.Exists(_gachaSavePath)) File.Delete(_gachaSavePath);

        QuestManager.Instance.ResetQuests(); // 퀘스트 리셋
        UnlockedCharacterIds.Clear(); // 잠금 해제된 캐릭터 목록 초기화
        Debug.Log("[GameManager] 게임 리셋 완료");
    }

    // GP 사용
    public void UseGP(int amount)
    {
        LoadedGP -= amount;
        QuestManager.Instance.currentGP = LoadedGP;
    }

    // SLock 값 설정 및 저장
    public void SetSLock(bool value)
    {
        LoadedSLock = value;
        SaveGachaDataOnly(); // 변경 즉시 저장
    }

    // 캐릭터 잠금 해제
    public void UnlockCharacter(string characterId)
    {
        if (!UnlockedCharacterIds.Contains(characterId))
        {
            UnlockedCharacterIds.Add(characterId);
            SaveGame(); // 게임 저장
        }
    }

    // 캐릭터 잠금 해제 여부 확인
    public bool IsCharacterUnlocked(string characterId)
    {
        return UnlockedCharacterIds.Contains(characterId);
    }
}
