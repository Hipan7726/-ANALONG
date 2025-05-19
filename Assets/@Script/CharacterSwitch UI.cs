using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// ĳ���� ��ȯ UI ���� Ŭ���� (�̱���)
/// ü�¹�, �ʻ�ȭ, �ؽ�Ʈ �� UI ��Ҹ� �����ϰ�, ��ȯ �ִϸ��̼ǵ� ó����
/// </summary>
public class CharacterSwitchUI : MonoBehaviour
{
    public static CharacterSwitchUI Instance; // �̱��� �ν��Ͻ�

    // UI ��ҵ�
    public Image characterPortrait1;
    public Image characterPortrait2;
    public Image healthBarFill1;
    public Image healthBarFill2;
    public TMP_Text healthText1;
    public TMP_Text healthText2;

    // �ΰ��� ĳ���� ���� (��Ʈ�ѷ����� ������)
    public InCharacter characterDataANBI;
    public InCharacter characterDataLONGINUS;

    // ��Ÿ�� ĳ���� ������
    private RuntimeCharacterData character1;
    private RuntimeCharacterData character2;
    private bool isCharacter1Active = true; // ���� Ȱ�� ĳ���Ͱ� character1���� ����

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // �ʿ�� �ʱ�ȭ �ڵ� ����
    }

    private void Update()
    {
        // �����̽� Ű�� ĳ���� ��ȯ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchCharacter(); // UI ���� ��ü
            StartCoroutine(ScalePortraitEffect()); // ��ȯ ����Ʈ
            InGameCharacterManager.Instance.SwapCharacter(); // ���� ĳ���� ��ȯ ó�� (�ܺ� �Ŵ���)
        }
    }

    /// <summary>
    /// ĳ���� ������ ����
    /// </summary>
    public void SetCharacters(RuntimeCharacterData data1, RuntimeCharacterData data2)
    {
        character1 = data1;
        character2 = data2;
        UpdateUI(); // ü�� �� UI ����
    }

    /// <summary>
    /// ĳ���� ��ȯ ó��
    /// </summary>
    public void SwitchCharacter()
    {
        isCharacter1Active = !isCharacter1Active; // Ȱ�� ĳ���� ����
        SwapUIReferences(); // UI ������Ʈ �� ���� (������ ����)
        UpdateUI(); // ���ο� ���� �ݿ�
    }

    /// <summary>
    /// ��Ʈ����Ʈ, ü�¹�, �ؽ�Ʈ UI ���� ����
    /// </summary>
    void SwapUIReferences()
    {
        (characterPortrait1, characterPortrait2) = (characterPortrait2, characterPortrait1);
        (healthBarFill1, healthBarFill2) = (healthBarFill2, healthBarFill1);
        (healthText1, healthText2) = (healthText2, healthText1);
    }

    /// <summary>
    /// ��ü UI ���� (�ʻ�ȭ + ü�¹�)
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
    /// ü�¹ٸ� �����ϰ� ���� �� ȣ�� (��: �ǰ� ��)
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
    /// ĳ���� UI �ϳ� ���� (�ʻ�ȭ + ü��)
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
    /// ü�¹� UI ����
    /// </summary>
    void SetHealthBar(RuntimeCharacterData data, Image healthFill, TMP_Text healthText)
    {
        healthFill.fillAmount = Mathf.Clamp01(data.HealthRatio);
        healthText.text = $"{data.CurrentHP} / {data.BaseData.maxHP}";
    }

    /// <summary>
    /// ���� ���� ���� ĳ���� ��Ʈ�ѷ����� UI ������ ���� ������ ����
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

        UpdateUI(); // ü�¹� �� ���ΰ�ħ
    }

    /// <summary>
    /// ��ȯ �� �ʻ�ȭ Ŀ���ٰ� �۾����� ȿ��
    /// </summary>
    public IEnumerator ScalePortraitEffect()
    {
        Image targetPortrait = isCharacter1Active ? characterPortrait1 : characterPortrait2;

        float duration = 0.5f;
        float timeElapsed = 0f;
        Vector3 originalScale = targetPortrait.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        // Ȯ��
        while (timeElapsed < duration / 2)
        {
            targetPortrait.transform.localScale = Vector3.Lerp(originalScale, targetScale, timeElapsed / (duration / 2));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // ���
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
    /// ���� ���� ���� ĳ���� ��ȯ
    /// </summary>
    public RuntimeCharacterData GetActiveCharacter() => isCharacter1Active ? character1 : character2;

    /// <summary>
    /// ��� ���� ĳ���� ��ȯ
    /// </summary>
    public RuntimeCharacterData GetStandbyCharacter() => isCharacter1Active ? character2 : character1;
}
