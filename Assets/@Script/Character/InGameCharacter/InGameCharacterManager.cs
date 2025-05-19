using System.Collections;
using UnityEngine;

public class InGameCharacterManager : MonoBehaviour
{
    public static InGameCharacterManager Instance;
    public event System.Action OnCharacterSwapped;

    private GameObject _currentCharacter;
    private bool _isUsingCharacter1 = true;

    [Header("ĳ���� ������")]
    public InCharacter CharacterDataANBI;
    public InCharacter CharacterDataLONGINUS;

    private RuntimeCharacterData _runtimeANBI;
    private RuntimeCharacterData _runtimeLONGINUS;

    [Header("UI ����")]
    public CharacterSwitchUI CharacterSwitchUI;

    [Header("ī�޶�")]
    public Camera PlayerCamera;

    [Header("���� ����")]
    public MonsterController MonsterController;  // ���� ��Ʈ�ѷ� �߰�


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // �� ĳ���� ������ ����
        _runtimeANBI = new RuntimeCharacterData(CharacterDataANBI);
        _runtimeLONGINUS = new RuntimeCharacterData(CharacterDataLONGINUS);

        // UI�� ����
        CharacterSwitchUI?.SetCharacters(_runtimeANBI, _runtimeLONGINUS);

        // �ʱ� ĳ���� ���� (ANBI ����)
        SpawnCharacter(CharacterDataANBI, _runtimeANBI);
        _isUsingCharacter1 = true;
    }

    public void SwapCharacter()
    {
        Vector3 spawnPosition = _currentCharacter.transform.position;
        Quaternion spawnRotation = _currentCharacter.transform.rotation;

        GameObject oldCharacter = _currentCharacter;

        // ��ü�� ��� ����
        InCharacter newCharacterData = _isUsingCharacter1 ? CharacterDataLONGINUS : CharacterDataANBI;

        // ����
        SpawnCharacter(newCharacterData, _isUsingCharacter1 ? _runtimeLONGINUS : _runtimeANBI, spawnPosition, spawnRotation);

        // ���� ĳ���� ����
        Destroy(oldCharacter);

        _isUsingCharacter1 = !_isUsingCharacter1;

        // ĳ���� ���� �� ���Ϳ��� �÷��̾� ��ü �˸�
        if (MonsterController != null) // MonsterController�� �Ҵ�Ǿ� �ִٸ�
        {
            MonsterController.UpdatePlayerTarget(_currentCharacter.transform); // ���ο� �÷��̾�� ����
        }

        // �̺�Ʈ ȣ�� (UI ��� ���)
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

    // ���� ĳ������ ������ (UI���� ���� ���� �� �ص� ��)
    public RuntimeCharacterData GetCurrentCharacterData()
    {
        return CharacterSwitchUI?.GetActiveCharacter();
    }

    // ��� ���� ĳ������ ������
    public RuntimeCharacterData GetStandbyCharacterData()
    {
        return CharacterSwitchUI?.GetStandbyCharacter();
    }


}
