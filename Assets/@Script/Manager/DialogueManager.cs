using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

/// <summary>
/// ���� �� ��ȭ �ý����� �����ϴ� �Ŵ��� Ŭ����
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;  // �̱��� �ν��Ͻ�

    [Header("UI")]
    public GameObject DialoguePanel;             // ��ȭâ UI �г�
    public TextMeshProUGUI SpeakerText;          // ȭ�� �̸� �ؽ�Ʈ
    public TextMeshProUGUI DialogueText;         // ��ȭ ���� �ؽ�Ʈ
    public float TypingSpeed = 0.02f;            // �� ���ھ� ��µǴ� �ӵ�

    private Queue<DialogueLine> _lines = new Queue<DialogueLine>(); // ���� ��� ���� ��� �ٵ�
    private bool _isTyping = false;               // ���� Ÿ���� ������ ����
    private bool _waitingForInput = false;        // ���� �Է� ��� ����

    private Dictionary<string, DialogueEntry> _dialogueDict = new(); // Ű ��� ��� ������ �����
    public bool IsInDialogue { get; private set; } = false;          // ���� ��ȭ ������ Ȯ��

    // JSON �Ľ��� ���� ���� Ŭ����
    [System.Serializable]
    private class DialogueWrapper
    {
        public List<DialogueEntry> entries;
    }

    private void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        DialoguePanel.SetActive(false);  // ���� �� ��ȭâ ��Ȱ��ȭ
        LoadDialogues();                 // JSON ���Ͽ��� ��� �ҷ�����
    }

    // StreamingAssets �������� JSON ��� ���� �ε�
    void LoadDialogues()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "dialogue.json");
        string json = File.ReadAllText(path);
        var dialogueList = JsonUtility.FromJson<DialogueWrapper>("{\"entries\":" + json + "}");

        foreach (var entry in dialogueList.entries)
        {
            _dialogueDict[entry.Key] = entry;  // key�� ���� �����ϰ� Dictionary�� ����
        }
    }

    // Ư�� Ű�� ��縦 �����ϴ� �޼���
    public void StartDialogue(string key, System.Action onComplete)
    {
        if (!_dialogueDict.ContainsKey(key))
        {
            Debug.LogWarning($"No dialogue for key: {key}");
            onComplete?.Invoke();  // ��簡 ���� ��쿡�� �ݹ��� ����
            return;
        }

        IsInDialogue = true;

        // ���콺 Ŀ�� ���̰� ����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DialoguePanel.SetActive(true);   // ��ȭ UI ����
        _lines.Clear();

        foreach (var line in _dialogueDict[key].Lines)
        {
            _lines.Enqueue(line);       // ��� ���� ť�� �߰�
        }

        StartCoroutine(ShowNextLine(onComplete));  // �ڷ�ƾ���� ��� ����
    }

    // ��縦 �� �پ� ����ϴ� �ڷ�ƾ
    System.Collections.IEnumerator ShowNextLine(System.Action onComplete)
    {
        while (_lines.Count > 0)
        {
            DialogueLine line = _lines.Dequeue();
            _isTyping = true;
            DialogueText.text = "";
            SpeakerText.text = line.Speaker;

            // �� ���ھ� ���
            foreach (char c in line.Text)
            {
                DialogueText.text += c;
                yield return new WaitForSeconds(TypingSpeed);
            }

            _isTyping = false;
            _waitingForInput = true;

            // Ŭ�� �Է��� ���� ������ ���
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            _waitingForInput = false;
        }

        // ��� ���� �� UI �ݰ� Ŀ�� ����
        DialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsInDialogue = false;
        onComplete?.Invoke();  // �Ϸ� �ݹ� ����
    }

    // ���� ���â�� ���� �ִ��� �ܺο��� Ȯ���ϴ� ������Ƽ
    public bool IsDialogueActive => DialoguePanel.activeSelf;
}
