using System.Collections.Generic;

// �� ���� ��� ������ ��� Ŭ����
[System.Serializable]
public class DialogueLine
{
    public string Speaker; // ��縦 ���ϴ� ȭ���� �̸�
    public string Text;    // ��� ����
}

// ��ȭ Ű �ϳ��� ���� ��ü ��� ���
[System.Serializable]
public class DialogueEntry
{
    public string Key;               // ��ȭ�� ���� Ű�� (��: NPC ID, ����Ʈ Ű ��)
    public List<DialogueLine> Lines; // �ش� Ű�� ���� ��ü ��� �� ���
}
