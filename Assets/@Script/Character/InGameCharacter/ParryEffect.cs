using UnityEngine;

public class ParryEffect : MonoBehaviour
{
    public static ParryEffect Instance; // 싱글톤 인스턴스

    public GameObject ParryEffectPrefab; // 패링 이펙트 프리팹
    private GameObject _currentEffect; // 현재 생성된 이펙트 오브젝트

    private void Awake()
    {
        // 싱글톤 패턴 - 이미 존재하면 파괴
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowParryEffect(Vector3 position)
    {
        if (ParryEffectPrefab == null)
        {
            Debug.LogWarning("ParryEffectPrefab is not assigned!"); // 프리팹 누락 경고
            return;
        }

        // 플레이어 위치 기준으로 머리 위에 이펙트 생성
        Vector3 effectPos = position + Vector3.up * 2f;

        // 이전 이펙트가 존재하면 제거
        if (_currentEffect != null)
        {
            Destroy(_currentEffect);
            _currentEffect = null;
        }

        // 새로운 이펙트 생성
        _currentEffect = Instantiate(ParryEffectPrefab, effectPos, Quaternion.identity);
    }

    public void DestroyEffect()
    {
        // 외부에서 수동으로 이펙트 제거할 때 사용
        Destroy(_currentEffect);
    }
}
