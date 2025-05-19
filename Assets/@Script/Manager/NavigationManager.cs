using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance; // 싱글톤 인스턴스

    public GameObject navigationDotPrefab; // 경로 표시를 위한 도트 프리팹
    public float spacing = 1.5f; // 도트 간격
    public int maxDots = 50;     // 최대 도트 수

    private List<GameObject> activeDots = new(); // 활성화된 도트 목록

    private void Awake()
    {
        if (Instance == null) Instance = this; // 싱글톤 설정
    }

    // 시작 지점(from)과 목표 지점(to) 사이에 경로를 생성하고 도트로 표시
    public void CreatePath(Vector3 from, Vector3 to)
    {
        ClearPath(); // 기존 경로 제거

        NavMeshPath path = new NavMeshPath(); // 경로를 저장할 객체

        // NavMesh를 사용해 경로 계산
        if (NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path))
        {
            Vector3 previousCorner = from;

            // 계산된 경로의 각 코너에 대해 처리
            for (int i = 1; i < path.corners.Length; i++)
            {
                Vector3 currentCorner = path.corners[i];
                float segmentDistance = Vector3.Distance(previousCorner, currentCorner); // 두 점 간 거리
                Vector3 segmentDirection = (currentCorner - previousCorner).normalized; // 방향 벡터

                // 두 점 간의 간격에 맞춰 도트 개수 계산
                int dotCount = Mathf.FloorToInt(segmentDistance / spacing);

                // 각 구간마다 도트 생성
                for (int j = 1; j <= dotCount; j++)
                {
                    Vector3 pos = previousCorner + segmentDirection * spacing * j; // 도트 위치 계산
                    GameObject dot = Instantiate(navigationDotPrefab, pos + Vector3.up * 0.1f, Quaternion.identity); // 도트 생성
                    activeDots.Add(dot); // 생성된 도트를 목록에 추가

                    // 도트 수가 최대치에 도달하면 더 이상 생성하지 않음
                    if (activeDots.Count >= maxDots)
                        return;
                }

                previousCorner = currentCorner; // 이전 코너를 현재 코너로 업데이트
            }
        }

        // 일정 시간 후 도트 제거
        StartCoroutine(RemoveDotsAfterSeconds(3f));
    }

    // 경로를 제거하는 함수
    public void ClearPath()
    {
        // 활성화된 도트들을 모두 제거
        foreach (var dot in activeDots)
        {
            Destroy(dot);
        }
        activeDots.Clear(); // 도트 목록 비우기
    }

    // 일정 시간이 지나면 도트를 자동으로 제거하는 코루틴
    private IEnumerator RemoveDotsAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds); // 지정된 시간 동안 대기
        ClearPath(); // 시간 지나면 경로 제거
    }
}
