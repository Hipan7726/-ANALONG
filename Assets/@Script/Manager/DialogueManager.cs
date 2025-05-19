using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

/// <summary>
/// 게임 내 대화 시스템을 관리하는 매니저 클래스
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;  // 싱글톤 인스턴스

    [Header("UI")]
    public GameObject DialoguePanel;             // 대화창 UI 패널
    public TextMeshProUGUI SpeakerText;          // 화자 이름 텍스트
    public TextMeshProUGUI DialogueText;         // 대화 내용 텍스트
    public float TypingSpeed = 0.02f;            // 한 글자씩 출력되는 속도

    private Queue<DialogueLine> _lines = new Queue<DialogueLine>(); // 현재 재생 중인 대사 줄들
    private bool _isTyping = false;               // 현재 타이핑 중인지 여부
    private bool _waitingForInput = false;        // 유저 입력 대기 상태

    private Dictionary<string, DialogueEntry> _dialogueDict = new(); // 키 기반 대사 데이터 저장소
    public bool IsInDialogue { get; private set; } = false;          // 현재 대화 중인지 확인

    // JSON 파싱을 위한 래퍼 클래스
    [System.Serializable]
    private class DialogueWrapper
    {
        public List<DialogueEntry> entries;
    }

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        DialoguePanel.SetActive(false);  // 시작 시 대화창 비활성화
        LoadDialogues();                 // JSON 파일에서 대사 불러오기
    }

    // StreamingAssets 폴더에서 JSON 대사 파일 로드
    void LoadDialogues()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "dialogue.json");
        string json = File.ReadAllText(path);
        var dialogueList = JsonUtility.FromJson<DialogueWrapper>("{\"entries\":" + json + "}");

        foreach (var entry in dialogueList.entries)
        {
            _dialogueDict[entry.Key] = entry;  // key로 접근 가능하게 Dictionary에 저장
        }
    }

    // 특정 키로 대사를 시작하는 메서드
    public void StartDialogue(string key, System.Action onComplete)
    {
        if (!_dialogueDict.ContainsKey(key))
        {
            Debug.LogWarning($"No dialogue for key: {key}");
            onComplete?.Invoke();  // 대사가 없을 경우에도 콜백은 실행
            return;
        }

        IsInDialogue = true;

        // 마우스 커서 보이게 설정
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DialoguePanel.SetActive(true);   // 대화 UI 열기
        _lines.Clear();

        foreach (var line in _dialogueDict[key].Lines)
        {
            _lines.Enqueue(line);       // 대사 줄을 큐에 추가
        }

        StartCoroutine(ShowNextLine(onComplete));  // 코루틴으로 대사 진행
    }

    // 대사를 한 줄씩 출력하는 코루틴
    System.Collections.IEnumerator ShowNextLine(System.Action onComplete)
    {
        while (_lines.Count > 0)
        {
            DialogueLine line = _lines.Dequeue();
            _isTyping = true;
            DialogueText.text = "";
            SpeakerText.text = line.Speaker;

            // 한 글자씩 출력
            foreach (char c in line.Text)
            {
                DialogueText.text += c;
                yield return new WaitForSeconds(TypingSpeed);
            }

            _isTyping = false;
            _waitingForInput = true;

            // 클릭 입력이 들어올 때까지 대기
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            _waitingForInput = false;
        }

        // 대사 종료 시 UI 닫고 커서 숨김
        DialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsInDialogue = false;
        onComplete?.Invoke();  // 완료 콜백 실행
    }

    // 현재 대사창이 열려 있는지 외부에서 확인하는 프로퍼티
    public bool IsDialogueActive => DialoguePanel.activeSelf;
}
