using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GachaManager : MonoBehaviour
{
    // ī�� �����հ� �θ� Ʈ������
    public GameObject CardPrefab;   // ī�� ������ (UI ī�� ��ü)
    public Transform CardParent;    // ī�尡 ������ �θ� Ʈ������

    // ī�� �̹�����
    public Sprite aFrontSprite;    // A�� ī�� �ո� �̹���
    public Sprite sFrontSprite;    // S�� ī�� �ո� �̹���
    public Sprite backSprite;      // ī�� �޸� �̹���

    // UI ��ҵ�
    public Button oneDrawButton;   // 1ȸ �̱� ��ư
    public Button tenDrawButton;   // 10ȸ �̱� ��ư
    public TMP_Text GpText;        // ���� GP �ؽ�Ʈ ǥ��
    public TMP_Text DrawCountText; // �̱� Ƚ�� �ؽ�Ʈ ǥ��
    public GameObject NoGP;        // GP ���� �� ��� UI
    public Button NoGPButton;      // GP ���� UI �ݱ� ��ư

    // ���� �� ���� ������
    private int _drawCount = 0;    // ��ü ���� Ƚ��
    private bool _isSCharacterDrawn = false;  // S�� ĳ���͸� �̹� �̾Ҵ��� ����
    private const float _defaultSRarityChance = 1f;  // �⺻ S�� Ȯ��
    private const float _boostedSRarityChance = 99f; // 80ȸ �̻� �̾��� �� S�� Ȯ�� ���

    private void Start()
    {
        // ���� ���� �� ��� ���� ���� �� ��í ȭ�� ���� ����
        SoundManager.Instance.BgmSource.Stop();
        SoundManager.Instance.PlayDrawScrene();

        // GP ���� UI�� ó���� ����
        NoGP.transform.localScale = Vector3.zero;

        // ��ư�� Ŭ�� ������ �߰�
        oneDrawButton.onClick.AddListener(() => DrawCards(1));  // 1ȸ �̱�
        tenDrawButton.onClick.AddListener(() => DrawCards(10));  // 10ȸ �̱�
        NoGPButton.onClick.AddListener(NoGpOK);  // GP ���� UI �ݱ� ��ư

        // ���� ���� �ε� (�̱� Ƚ��, GP)
        _drawCount = GameManager.Instance.LoadedDrawCount;
        GameManager.Instance.UseGP(0);  // GP ���� ���� �ʱ�ȭ

        GpText.text = $"{GameManager.Instance.LoadedGP}";  // ���� GP ǥ��
        DrawCountText.text = $"�̱� Ƚ�� : {_drawCount}";  // ���� �̱� Ƚ�� ǥ��
    }

    void Update()
    {
        if (AllCardsFlipped() && Input.GetMouseButtonDown(0))
        {
            ClearCards();
        }
    }

    // ī�带 �̴� ����
    void DrawCards(int count)
    {
        SoundManager.Instance.PlayButton();  // ��ư Ŭ�� �Ҹ�

        int cost = count == 1 ? 160 : 1600;  // 1ȸ �̱�� 10ȸ �̱��� ���

        // GP�� ������ ���
        if (GameManager.Instance.LoadedGP < cost)
        {
            // GP ���� UI Ȱ��ȭ
            StartCoroutine(ScaleUI(NoGP, Vector3.zero, Vector3.one, 0.1f));
            Debug.Log("GP�� �����մϴ�.");
            return;
        }

        // ��븸ŭ GP ����
        GameManager.Instance.UseGP(cost);
        GpText.text = $"{GameManager.Instance.LoadedGP}";  // GP ������Ʈ
        GameManager.Instance.SaveGame();  // ���� ���� ����

        ClearCards();  // ���� ī��� ����

        bool sDrawnThisBatch = false;  // �̹� �̱⿡�� S�� ī�尡 ���Դ��� Ȯ���ϴ� ����

        // ���õ� Ƚ����ŭ ī�� �̱�
        for (int i = 0; i < count; i++)
        {
            int currentDrawCount = _drawCount + 1;  // �̹� �̱��� ����

            // S�� ī���� Ȯ���� ����
            float sChance = (!_isSCharacterDrawn && !sDrawnThisBatch && currentDrawCount >= 80)
                ? _boostedSRarityChance  // 80��° ���Ŀ��� S�� Ȯ�� ����
                : _defaultSRarityChance; // �⺻ S�� Ȯ��

            // ���� ���� ���� (0~100 ����)
            float roll = Random.Range(0f, 100f);
            Rarity rarity = roll < sChance ? Rarity.S : Rarity.A;  // Ȯ���� ���� S�� �Ǵ� A�� ����

            // ī�� ����
            GameObject cardObj = Instantiate(CardPrefab, CardParent);
            GachaCard card = cardObj.GetComponent<GachaCard>();

            // A�� ī�� ����
            if (rarity == Rarity.A)
            {
                card.SetCard(aFrontSprite, backSprite, () =>
                {
                    Debug.Log("A: ��");
                });
            }
            // S�� ī�� ����
            else if (rarity == Rarity.S)
            {
                card.SetCard(sFrontSprite, backSprite, () =>
                {
                    // S�� ĳ���� ��� ����
                    GameManager.Instance.SetSLock(true);

                    _isSCharacterDrawn = true;  // S���� �̾����� ���
                    sDrawnThisBatch = true;     // �̹� �̱⿡�� S���� �̾����� ���
                });
            }

            _drawCount++;  // �̱� Ƚ�� ����
        }

        // S���� �̾����� ��ü �̱� Ƚ�� �ʱ�ȭ
        if (sDrawnThisBatch)
        {
            _drawCount = 0;  // S���� �̾����Ƿ� Ƚ�� �ʱ�ȭ
            Debug.Log("S���� �̾����Ƿ� ī��Ʈ �ʱ�ȭ");
        }

        // ���� ���� ����
        GameManager.Instance.LoadedDrawCount = _drawCount;
        GameManager.Instance.SaveGame();

        // UI ����
        DrawCountText.text = $"�̱� Ƚ�� : {_drawCount}";
    }

    // ī��� Ŭ���� (���� ī��� ����)
    void ClearCards()
    {
        foreach (Transform child in CardParent)
        {
            Destroy(child.gameObject);  // �θ� ������Ʈ���� �ڽ� ī��� ����
        }
    }

    // ī�� ��� (A��, S��)
    enum Rarity { A, S }

    // GP ���� UI �ݱ�
    void NoGpOK()
    {
        StartCoroutine(ScaleUI(NoGP, Vector3.one, Vector3.zero, 0.1f, true));  // UI ��� �� ��Ȱ��ȭ
    }

    // UI ��� ũ�� ���� �ִϸ��̼� (��: GP ���� UI)
    IEnumerator ScaleUI(GameObject obj, Vector3 fromScale, Vector3 toScale, float duration, bool deactivateOnEnd = false)
    {
        float time = 0f;
        obj.SetActive(true);  // UI Ȱ��ȭ

        // ũ�� �ִϸ��̼�
        while (time < duration)
        {
            obj.transform.localScale = Vector3.Lerp(fromScale, toScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = toScale;  // ���� ũ�� ����

        // �ִϸ��̼� ���� �� ��Ȱ��ȭ
        if (deactivateOnEnd)
            obj.SetActive(false);
    }

    // ��� ī�尡 ���������� Ȯ�� (ī�� ������ �Ϸ� Ȯ��)
    bool AllCardsFlipped()
    {
        foreach (Transform child in CardParent)
        {
            GachaCard card = child.GetComponent<GachaCard>();
            if (card != null && !card.IsFlipped())
            {
                return false;  // ���� �������� ���� ī�尡 ������ false ��ȯ
            }
        }
        return true;  // ��� ī�尡 ���������� true ��ȯ
    }
}
