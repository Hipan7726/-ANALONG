using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 캐릭터 전환 UI 관리 클래스 (싱글톤)
/// 체력바, 초상화, 텍스트 등 UI 요소를 갱신하고, 전환 애니메이션도 처리함
/// </summary>
public class CharacterSwitchUI : MonoBehaviour
{
    public static CharacterSwitchUI Instance; // 싱글톤 인스턴스

    // UI 요소들
    public Image characterPortrait1;
    public Image characterPortrait2;
    public Image healthBarFill1;
    public Image healthBarFill2;
    public TMP_Text healthText1;
    public TMP_Text healthText2;

    // 인게임 캐릭터 참조 (컨트롤러에서 설정됨)
    public InCharacter characterDataANBI;
    public InCharacter characterDataLONGINUS;

    // 런타임 캐릭터 데이터
    private RuntimeCharacterData character1;
    private RuntimeCharacterData character2;
    private bool isCharacter1Active = true; // 현재 활성 캐릭터가 character1인지 여부

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // 필요시 초기화 코드 가능
    }

    private void Update()
    {
        // 스페이스 키로 캐릭터 전환
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchCharacter(); // UI 내부 교체
            StartCoroutine(ScalePortraitEffect()); // 전환 이펙트
            InGameCharacterManager.Instance.SwapCharacter(); // 실제 캐릭터 전환 처리 (외부 매니저)
        }
    }

    /// <summary>
    /// 캐릭터 데이터 설정
    /// </summary>
    public void SetCharacters(RuntimeCharacterData data1, RuntimeCharacterData data2)
    {
        character1 = data1;
        character2 = data2;
        UpdateUI(); // 체력 등 UI 갱신
    }

    /// <summary>
    /// 캐릭터 전환 처리
    /// </summary>
    public void SwitchCharacter()
    {
        isCharacter1Active = !isCharacter1Active; // 활성 캐릭터 스왑
        SwapUIReferences(); // UI 오브젝트 간 스왑 (참조만 변경)
        UpdateUI(); // 새로운 상태 반영
    }

    /// <summary>
    /// 포트레이트, 체력바, 텍스트 UI 참조 스왑
    /// </summary>
    void SwapUIReferences()
    {
        (characterPortrait1, characterPortrait2) = (characterPortrait2, characterPortrait1);
        (healthBarFill1, healthBarFill2) = (healthBarFill2, healthBarFill1);
        (healthText1, healthText2) = (healthText2, healthText1);
    }

    /// <summary>
    /// 전체 UI 갱신 (초상화 + 체력바)
    /// </summary>
    void UpdateUI()
    {
        if (isCharacter1Active)
        {
            SetUI(character1, characterPortrait1, healthBarFill1, healthText1);
            SetUI(character2, characterPortrait2, healthBarFill2, healthText2);
        }
        else
        {
            SetUI(character2, characterPortrait2, healthBarFill2, healthText2);
            SetUI(character1, characterPortrait1, healthBarFill1, healthText1);
        }
    }

    /// <summary>
    /// 체력바만 갱신하고 싶을 때 호출 (예: 피격 시)
    /// </summary>
    public void UpdateHealthBarOnly()
    {
        if (isCharacter1Active)
        {
            SetHealthBar(character1, healthBarFill1, healthText1);
            SetHealthBar(character2, healthBarFill2, healthText2);
        }
        else
        {
            SetHealthBar(character2, healthBarFill2, healthText2);
            SetHealthBar(character1, healthBarFill1, healthText1);
        }
    }

    /// <summary>
    /// 캐릭터 UI 하나 설정 (초상화 + 체력)
    /// </summary>
    void SetUI(RuntimeCharacterData data, Image portrait, Image healthFill, TMP_Text healthText)
    {
        if (data != null && data.BaseData != null)
        {
            portrait.sprite = data.BaseData.characterImage;
            SetHealthBar(data, healthFill, healthText);
        }
    }

    /// <summary>
    /// 체력바 UI 설정
    /// </summary>
    void SetHealthBar(RuntimeCharacterData data, Image healthFill, TMP_Text healthText)
    {
        healthFill.fillAmount = Mathf.Clamp01(data.HealthRatio);
        healthText.text = $"{data.CurrentHP} / {data.BaseData.maxHP}";
    }

    /// <summary>
    /// 현재 조작 중인 캐릭터 컨트롤러에서 UI 연동을 위한 데이터 설정
    /// </summary>
    public void SetCurrentCharacter(MonoBehaviour controller)
    {
        if (controller is ZZZCharacterControllerANBI anbi)
        {
            characterDataANBI = anbi.InCharacter;
        }
        else if (controller is ZZZCharacterControllerLONGINUS longinus)
        {
            characterDataLONGINUS = longinus.InCharacter;
        }

        UpdateUI(); // 체력바 등 새로고침
    }

    /// <summary>
    /// 전환 시 초상화 커졌다가 작아지는 효과
    /// </summary>
    public IEnumerator ScalePortraitEffect()
    {
        Image targetPortrait = isCharacter1Active ? characterPortrait1 : characterPortrait2;

        float duration = 0.5f;
        float timeElapsed = 0f;
        Vector3 originalScale = targetPortrait.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        // 확대
        while (timeElapsed < duration / 2)
        {
            targetPortrait.transform.localScale = Vector3.Lerp(originalScale, targetScale, timeElapsed / (duration / 2));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // 축소
        timeElapsed = 0f;
        while (timeElapsed < duration / 2)
        {
            targetPortrait.transform.localScale = Vector3.Lerp(targetScale, originalScale, timeElapsed / (duration / 2));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        targetPortrait.transform.localScale = originalScale;
    }

    /// <summary>
    /// 현재 조작 중인 캐릭터 반환
    /// </summary>
    public RuntimeCharacterData GetActiveCharacter() => isCharacter1Active ? character1 : character2;

    /// <summary>
    /// 대기 중인 캐릭터 반환
    /// </summary>
    public RuntimeCharacterData GetStandbyCharacter() => isCharacter1Active ? character2 : character1;
}
