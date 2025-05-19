using System.Collections.Generic;

// 한 줄의 대사 정보를 담는 클래스
[System.Serializable]
public class DialogueLine
{
    public string Speaker; // 대사를 말하는 화자의 이름
    public string Text;    // 대사 내용
}

// 대화 키 하나에 대한 전체 대사 목록
[System.Serializable]
public class DialogueEntry
{
    public string Key;               // 대화의 고유 키값 (예: NPC ID, 퀘스트 키 등)
    public List<DialogueLine> Lines; // 해당 키에 대한 전체 대사 줄 목록
}
