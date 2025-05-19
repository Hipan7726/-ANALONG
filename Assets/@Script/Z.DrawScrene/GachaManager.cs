using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GachaManager : MonoBehaviour
{
    // 카드 프리팹과 부모 트랜스폼
    public GameObject CardPrefab;   // 카드 프리팹 (UI 카드 객체)
    public Transform CardParent;    // 카드가 생성될 부모 트랜스폼

    // 카드 이미지들
    public Sprite aFrontSprite;    // A급 카드 앞면 이미지
    public Sprite sFrontSprite;    // S급 카드 앞면 이미지
    public Sprite backSprite;      // 카드 뒷면 이미지

    // UI 요소들
    public Button oneDrawButton;   // 1회 뽑기 버튼
    public Button tenDrawButton;   // 10회 뽑기 버튼
    public TMP_Text GpText;        // 현재 GP 텍스트 표시
    public TMP_Text DrawCountText; // 뽑기 횟수 텍스트 표시
    public GameObject NoGP;        // GP 부족 시 경고 UI
    public Button NoGPButton;      // GP 부족 UI 닫기 버튼

    // 게임 내 상태 변수들
    private int _drawCount = 0;    // 전체 뽑은 횟수
    private bool _isSCharacterDrawn = false;  // S급 캐릭터를 이미 뽑았는지 여부
    private const float _defaultSRarityChance = 1f;  // 기본 S급 확률
    private const float _boostedSRarityChance = 99f; // 80회 이상 뽑았을 때 S급 확률 상승

    private void Start()
    {
        // 게임 시작 시 배경 음악 중지 및 가챠 화면 음악 시작
        SoundManager.Instance.BgmSource.Stop();
        SoundManager.Instance.PlayDrawScrene();

        // GP 부족 UI는 처음에 숨김
        NoGP.transform.localScale = Vector3.zero;

        // 버튼에 클릭 리스너 추가
        oneDrawButton.onClick.AddListener(() => DrawCards(1));  // 1회 뽑기
        tenDrawButton.onClick.AddListener(() => DrawCards(10));  // 10회 뽑기
        NoGPButton.onClick.AddListener(NoGpOK);  // GP 부족 UI 닫기 버튼

        // 게임 상태 로드 (뽑기 횟수, GP)
        _drawCount = GameManager.Instance.LoadedDrawCount;
        GameManager.Instance.UseGP(0);  // GP 차감 없이 초기화

        GpText.text = $"{GameManager.Instance.LoadedGP}";  // 현재 GP 표시
        DrawCountText.text = $"뽑기 횟수 : {_drawCount}";  // 현재 뽑기 횟수 표시
    }

    void Update()
    {
        if (AllCardsFlipped() && Input.GetMouseButtonDown(0))
        {
            ClearCards();
        }
    }

    // 카드를 뽑는 로직
    void DrawCards(int count)
    {
        SoundManager.Instance.PlayButton();  // 버튼 클릭 소리

        int cost = count == 1 ? 160 : 1600;  // 1회 뽑기와 10회 뽑기의 비용

        // GP가 부족한 경우
        if (GameManager.Instance.LoadedGP < cost)
        {
            // GP 부족 UI 활성화
            StartCoroutine(ScaleUI(NoGP, Vector3.zero, Vector3.one, 0.1f));
            Debug.Log("GP가 부족합니다.");
            return;
        }

        // 비용만큼 GP 차감
        GameManager.Instance.UseGP(cost);
        GpText.text = $"{GameManager.Instance.LoadedGP}";  // GP 업데이트
        GameManager.Instance.SaveGame();  // 게임 상태 저장

        ClearCards();  // 이전 카드들 제거

        bool sDrawnThisBatch = false;  // 이번 뽑기에서 S급 카드가 나왔는지 확인하는 변수

        // 선택된 횟수만큼 카드 뽑기
        for (int i = 0; i < count; i++)
        {
            int currentDrawCount = _drawCount + 1;  // 이번 뽑기의 순번

            // S급 카드의 확률을 결정
            float sChance = (!_isSCharacterDrawn && !sDrawnThisBatch && currentDrawCount >= 80)
                ? _boostedSRarityChance  // 80번째 이후에는 S급 확률 증가
                : _defaultSRarityChance; // 기본 S급 확률

            // 랜덤 숫자 생성 (0~100 범위)
            float roll = Random.Range(0f, 100f);
            Rarity rarity = roll < sChance ? Rarity.S : Rarity.A;  // 확률에 따라 S급 또는 A급 결정

            // 카드 생성
            GameObject cardObj = Instantiate(CardPrefab, CardParent);
            GachaCard card = cardObj.GetComponent<GachaCard>();

            // A급 카드 설정
            if (rarity == Rarity.A)
            {
                card.SetCard(aFrontSprite, backSprite, () =>
                {
                    Debug.Log("A: 꽝");
                });
            }
            // S급 카드 설정
            else if (rarity == Rarity.S)
            {
                card.SetCard(sFrontSprite, backSprite, () =>
                {
                    // S급 캐릭터 잠금 해제
                    GameManager.Instance.SetSLock(true);

                    _isSCharacterDrawn = true;  // S급을 뽑았음을 기록
                    sDrawnThisBatch = true;     // 이번 뽑기에서 S급을 뽑았음을 기록
                });
            }

            _drawCount++;  // 뽑기 횟수 증가
        }

        // S급을 뽑았으면 전체 뽑기 횟수 초기화
        if (sDrawnThisBatch)
        {
            _drawCount = 0;  // S급을 뽑았으므로 횟수 초기화
            Debug.Log("S급을 뽑았으므로 카운트 초기화");
        }

        // 게임 상태 저장
        GameManager.Instance.LoadedDrawCount = _drawCount;
        GameManager.Instance.SaveGame();

        // UI 갱신
        DrawCountText.text = $"뽑기 횟수 : {_drawCount}";
    }

    // 카드들 클리어 (기존 카드들 삭제)
    void ClearCards()
    {
        foreach (Transform child in CardParent)
        {
            Destroy(child.gameObject);  // 부모 오브젝트에서 자식 카드들 삭제
        }
    }

    // 카드 등급 (A급, S급)
    enum Rarity { A, S }

    // GP 부족 UI 닫기
    void NoGpOK()
    {
        StartCoroutine(ScaleUI(NoGP, Vector3.one, Vector3.zero, 0.1f, true));  // UI 축소 후 비활성화
    }

    // UI 요소 크기 변경 애니메이션 (예: GP 부족 UI)
    IEnumerator ScaleUI(GameObject obj, Vector3 fromScale, Vector3 toScale, float duration, bool deactivateOnEnd = false)
    {
        float time = 0f;
        obj.SetActive(true);  // UI 활성화

        // 크기 애니메이션
        while (time < duration)
        {
            obj.transform.localScale = Vector3.Lerp(fromScale, toScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = toScale;  // 최종 크기 설정

        // 애니메이션 종료 후 비활성화
        if (deactivateOnEnd)
            obj.SetActive(false);
    }

    // 모든 카드가 뒤집혔는지 확인 (카드 뒤집기 완료 확인)
    bool AllCardsFlipped()
    {
        foreach (Transform child in CardParent)
        {
            GachaCard card = child.GetComponent<GachaCard>();
            if (card != null && !card.IsFlipped())
            {
                return false;  // 아직 뒤집히지 않은 카드가 있으면 false 반환
            }
        }
        return true;  // 모든 카드가 뒤집혔으면 true 반환
    }
}
