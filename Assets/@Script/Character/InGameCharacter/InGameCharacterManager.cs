using System.Collections;
using UnityEngine;

public class InGameCharacterManager : MonoBehaviour
{
    public static InGameCharacterManager Instance;
    public event System.Action OnCharacterSwapped;

    private GameObject _currentCharacter;
    private bool _isUsingCharacter1 = true;

    [Header("캐릭터 데이터")]
    public InCharacter CharacterDataANBI;
    public InCharacter CharacterDataLONGINUS;

    private RuntimeCharacterData _runtimeANBI;
    private RuntimeCharacterData _runtimeLONGINUS;

    [Header("UI 연동")]
    public CharacterSwitchUI CharacterSwitchUI;

    [Header("카메라")]
    public Camera PlayerCamera;

    [Header("몬스터 연동")]
    public MonsterController MonsterController;  // 몬스터 컨트롤러 추가


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // 각 캐릭터 데이터 생성
        _runtimeANBI = new RuntimeCharacterData(CharacterDataANBI);
        _runtimeLONGINUS = new RuntimeCharacterData(CharacterDataLONGINUS);

        // UI에 전달
        CharacterSwitchUI?.SetCharacters(_runtimeANBI, _runtimeLONGINUS);

        // 초기 캐릭터 스폰 (ANBI 먼저)
        SpawnCharacter(CharacterDataANBI, _runtimeANBI);
        _isUsingCharacter1 = true;
    }

    public void SwapCharacter()
    {
        Vector3 spawnPosition = _currentCharacter.transform.position;
        Quaternion spawnRotation = _currentCharacter.transform.rotation;

        GameObject oldCharacter = _currentCharacter;

        // 교체할 대상 결정
        InCharacter newCharacterData = _isUsingCharacter1 ? CharacterDataLONGINUS : CharacterDataANBI;

        // 스폰
        SpawnCharacter(newCharacterData, _isUsingCharacter1 ? _runtimeLONGINUS : _runtimeANBI, spawnPosition, spawnRotation);

        // 기존 캐릭터 삭제
        Destroy(oldCharacter);

        _isUsingCharacter1 = !_isUsingCharacter1;

        // 캐릭터 스왑 후 몬스터에게 플레이어 교체 알림
        if (MonsterController != null) // MonsterController가 할당되어 있다면
        {
            MonsterController.UpdatePlayerTarget(_currentCharacter.transform); // 새로운 플레이어로 갱신
        }

        // 이벤트 호출 (UI 등에서 사용)
        OnCharacterSwapped?.Invoke();
    }

    private void SpawnCharacter(InCharacter data, RuntimeCharacterData runtimeData, Vector3? position = null, Quaternion? rotation = null)
    {
        Vector3 spawnPosition = position ?? transform.position;
        Quaternion spawnRotation = rotation ?? Quaternion.Euler(0, PlayerCamera.transform.eulerAngles.y + 90f, 0);

        _currentCharacter = Instantiate(data.characterPrefab, spawnPosition, spawnRotation);
        _currentCharacter.SetActive(true);

        if (_currentCharacter.TryGetComponent(out ZZZCharacterControllerANBI controllerANBI))
        {
            controllerANBI.Initialize(runtimeData);
        }
        else if (_currentCharacter.TryGetComponent(out ZZZCharacterControllerLONGINUS controllerLONGINUS))
        {
            controllerLONGINUS.Initialize(runtimeData);
        }

        PlayerCamera.GetComponent<ThirdPersonCamera>()?.SetTarget(_currentCharacter.transform);
    }

    // 현재 캐릭터의 데이터 (UI에서 직접 접근 안 해도 됨)
    public RuntimeCharacterData GetCurrentCharacterData()
    {
        return CharacterSwitchUI?.GetActiveCharacter();
    }

    // 대기 중인 캐릭터의 데이터
    public RuntimeCharacterData GetStandbyCharacterData()
    {
        return CharacterSwitchUI?.GetStandbyCharacter();
    }


}
