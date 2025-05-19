using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterListManager : MonoBehaviour
{

    [Header("ĳ���� ����")]
    public List<CharacterDataSO> ownedCharacters;

    [Header("UI ������Ʈ")]
    public GameObject[] cardSlots; // ī�� ���� (����)
    public GameObject characterDisplayArea; // ������ ���� ��ġ (����)
    public TextMeshProUGUI characterNameText; // ���õ� ĳ���� �̸� ǥ�� �ؽ�Ʈ
    public Sprite emptyCardSprite; // �� ���� �̹���
    public GameObject UnLock;

    //ĳ���� �µθ�
    public List<GameObject> selectionBorders; // �׵θ� �̹��� ����Ʈ (�ε��� �������)
    private int selectedCharacterIndex = -1;

    //ī�޶� ���� ĳ����
    public Camera characterCamera;

    //������ ����
    public Button ProfileButtonA;               //������ ��ư
    public GameObject ProfileImage;             //������ â     
    public TextMeshProUGUI Name;
    public TextMeshProUGUI HP;                  //ü��
    public TextMeshProUGUI Damage;              //������
    public TextMeshProUGUI Penetratingpower;    //�����
    public TextMeshProUGUI CriticalHit;         //ġ��Ÿ Ȯ��
    public TextMeshProUGUI CriticalHitDamage;   //ġ��Ÿ ����
    
    //Back��ư
    public Button BackButton;
    private bool hasFaded = false;

    private GameObject currentCharacterInstance;


    void Start()
    {
        UpdateCardSlots();
        ProfileButtonA.onClick.AddListener(ProfileButton);
        BackButton.onClick.AddListener(Back);
        ProfileImage.SetActive(false);

        //���콺 Ŀ��
        Cursor.visible = true;                     // ���콺 Ŀ�� ���̰�
        Cursor.lockState = CursorLockMode.None;    // Ŀ�� ��� ����

        if(GameManager.Instance.LoadedSLock == true)
        {
            UnLock.SetActive(false);
        }
    }

    //
    void UpdateCardSlots()
    {
        for (int i = 0; i < cardSlots.Length; i++)
        {
            Image img = cardSlots[i].GetComponent<Image>();
            Button btn = cardSlots[i].GetComponent<Button>();
            TextMeshProUGUI nameText = cardSlots[i].GetComponentInChildren<TextMeshProUGUI>();

            btn.onClick.RemoveAllListeners(); // �ߺ� ����

            if (i < ownedCharacters.Count)
            {
                var character = ownedCharacters[i];
                img.sprite = character.characterImage;

                if (nameText != null)
                    nameText.text = character.characterName;

                int index = i; // ĸó ����
                btn.onClick.AddListener(() => ShowCharacter(index));
            }
            else
            {
                img.sprite = emptyCardSprite;

                if (nameText != null)
                    nameText.text = "";

                btn.onClick.RemoveAllListeners();
            }
        }
    }

    void ShowCharacter(int index)
    {
        selectedCharacterIndex = index;
        UpdateSelectionBorders(index); // �׵θ� ������Ʈ

        if (currentCharacterInstance != null)
            Destroy(currentCharacterInstance);

        Vector3 spawnPos = characterDisplayArea.transform.position;
        Quaternion rotation = Quaternion.Euler(0, 160, 0);

        currentCharacterInstance = Instantiate(
            ownedCharacters[index].characterPrefab,
            spawnPos,
            rotation,
            characterDisplayArea.transform
        );

        if (characterNameText != null)
            characterNameText.text = ownedCharacters[index].characterName;

        //ĳ���� ������ �κ�
        Name.text = ownedCharacters[index].characterName;
        HP.text = ownedCharacters[index].hp.ToString();
        Damage.text = ownedCharacters[index].damage.ToString();
        Penetratingpower.text = ownedCharacters[index].penetratingpower.ToString();
        CriticalHit.text = ownedCharacters[index].criticalHit.ToString();
        CriticalHitDamage.text = ownedCharacters[index].criticalHitDamage.ToString();

        // ���� ���
        if (ownedCharacters[index].voiceClip != null)
            SoundManager.Instance.PlayCharacterSource(ownedCharacters[index].voiceClip);

    }

        void UpdateSelectionBorders(int selectedIndex)
    {
        for (int i = 0; i < selectionBorders.Count; i++)
        {
            selectionBorders[i].SetActive(i == selectedIndex);
        }
    }

    void ProfileButton()
    {
        SoundManager.Instance.PlayButton();

        ProfileImage.SetActive(true);

        StartCoroutine(CameraTransition(
            new Vector3(-5.64f, -6.4f, -7.72f),
            Quaternion.Euler(-47.8f, 42.4f, -24.77f),
            40f,
            0.3f // ��ȯ �ð�
        ));
    }

    void Back()
    {
        SoundManager.Instance.PlayButton();

        if (ProfileImage.activeSelf)
        {
            ProfileImage.SetActive(false);

            StartCoroutine(CameraTransition(
                new Vector3(0, 0, -12),
                Quaternion.Euler(0, 0, 0),
                60f,
                0.3f
            ));
        }
        else
        {
            hasFaded = true;
            StartCoroutine(LoadWithFade());
        }
    }

    public void RefreshUI()
    {
        UpdateCardSlots();
        if (selectedCharacterIndex >= 0)
        {
            ShowCharacter(selectedCharacterIndex);
        }
    }


    IEnumerator LoadWithFade()
    {
        yield return FadeManager.Instance.FadeOut();

        SceneManager.LoadScene("StartGame");
    }

    IEnumerator CameraTransition(Vector3 targetPosition, Quaternion targetRotation, float targetFOV, float duration = 1f)
    {
        Vector3 startPos = characterCamera.transform.position;
        Quaternion startRot = characterCamera.transform.rotation;
        float startFOV = characterCamera.fieldOfView;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            characterCamera.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            characterCamera.transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);
            characterCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            yield return null;
        }

        // ���� ��ġ ����
        characterCamera.transform.position = targetPosition;
        characterCamera.transform.rotation = targetRotation;
        characterCamera.fieldOfView = targetFOV;
    }

}
