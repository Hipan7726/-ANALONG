using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 개별 퀘스트 정보를 담는 클래스
/// </summary>
[System.Serializable]
public class Quest
{
    public string id;               // 퀘스트 고유 ID
    public string title;            // 퀘스트 제목
    public string description;      // 퀘스트 설명
    public string triggerType;      // 퀘스트 완료 조건 타입 (예: "TalkToNPC", "CollectItem" 등)
    public string target;           // 퀘스트 목표 대상 (예: 특정 NPC 이름, 아이템 이름 등)
    public string nextQuestId;      // 이 퀘스트 완료 후 자동으로 시작될 다음 퀘스트 ID
    public QuestReward rewards;     // 퀘스트 완료 시 지급되는 보상

    public Vector3Serializable targetPosition;  // 추가
}

    /// <summary>
    /// 퀘스트 보상 정보를 담는 클래스
    /// </summary>
    [System.Serializable]
public class QuestReward
{
    public int gold;                // 획득 골드
    public int gp;                  // 획득 경험치 또는 게임 포인트
}

/// <summary>
/// JSON 파싱을 위한 퀘스트 리스트 래퍼 클래스
/// </summary>
[System.Serializable]
public class QuestList
{
    public List<Quest> quests;      // 전체 퀘스트 목록
}

[System.Serializable]
public class Vector3Serializable
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3() => new(x, y, z);
}

