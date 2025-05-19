using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterListManager : MonoBehaviour
{

    [Header("캐릭터 정보")]
    public List<CharacterDataSO> ownedCharacters;

    [Header("UI 오브젝트")]
    public GameObject[] cardSlots; // 카드 슬롯 (우측)
    public GameObject characterDisplayArea; // 프리팹 스폰 위치 (좌측)
    public TextMeshProUGUI characterNameText; // 선택된 캐릭터 이름 표시 텍스트
    public Sprite emptyCardSprite; // 빈 슬롯 이미지
    public GameObject UnLock;

    //캐릭터 태두리
    public List<GameObject> selectionBorders; // 테두리 이미지 리스트 (인덱스 순서대로)
    private int selectedCharacterIndex = -1;

    //카메라 설정 캐릭터
    public Camera characterCamera;

    //프로필 관련
    public Button ProfileButtonA;               //프로필 버튼
    public GameObject ProfileImage;             //프로필 창     
    public TextMeshProUGUI Name;
    public TextMeshProUGUI HP;                  //체력
    public TextMeshProUGUI Damage;              //데미지
    public TextMeshProUGUI Penetratingpower;    //관통력
    public TextMeshProUGUI CriticalHit;         //치명타 확률
    public TextMeshProUGUI CriticalHitDamage;   //치명타 피해
    
    //Back버튼
    public Button BackButton;
    private bool hasFaded = false;

    private GameObject currentCharacterInstance;


    void Start()
    {
        UpdateCardSlots();
        ProfileButtonA.onClick.AddListener(ProfileButton);
        BackButton.onClick.AddListener(Back);
        ProfileImage.SetActive(false);

        //마우스 커서
        Cursor.visible = true;                     // 마우스 커서 보이게
        Cursor.lockState = CursorLockMode.None;    // 커서 잠금 해제

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

            btn.onClick.RemoveAllListeners(); // 중복 방지

            if (i < ownedCharacters.Count)
            {
                var character = ownedCharacters[i];
                img.sprite = character.characterImage;

                if (nameText != null)
                    nameText.text = character.characterName;

                int index = i; // 캡처 변수
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
        UpdateSelectionBorders(index); // 테두리 업데이트

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

        //캐릭터 프로필 부분
        Name.text = ownedCharacters[index].characterName;
        HP.text = ownedCharacters[index].hp.ToString();
        Damage.text = ownedCharacters[index].damage.ToString();
        Penetratingpower.text = ownedCharacters[index].penetratingpower.ToString();
        CriticalHit.text = ownedCharacters[index].criticalHit.ToString();
        CriticalHitDamage.text = ownedCharacters[index].criticalHitDamage.ToString();

        // 음성 재생
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
            0.3f // 전환 시간
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

        // 최종 위치 보정
        characterCamera.transform.position = targetPosition;
        characterCamera.transform.rotation = targetRotation;
        characterCamera.fieldOfView = targetFOV;
    }

}
