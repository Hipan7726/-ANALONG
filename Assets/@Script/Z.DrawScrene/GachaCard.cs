using UnityEngine;
using UnityEngine.UI;

public class GachaCard : MonoBehaviour
{
    public Image frontImage;  // ī�� �ո� �̹���
    public Image backImage;   // ī�� �޸� �̹���
    public Button cardButton; // ī�� Ŭ�� ��ư

    private bool isFlipped = false;  // ī�尡 ���������� ����
    private System.Action onRevealAction;  // ī�� ������ �� ����� �ݹ�

    // ī���� �ո�� �޸� �̹����� �����ϰ�, ī�� ������ �� ������ �ݹ� ����
    public void SetCard(Sprite frontSprite, Sprite backSprite, System.Action onReveal)
    {
        frontImage.sprite = frontSprite;  // ī�� �ո� ��������Ʈ ����
        backImage.sprite = backSprite;    // ī�� �޸� ��������Ʈ ����

        frontImage.gameObject.SetActive(false);  // ó������ �ո��� ������ �ʵ��� ����
        backImage.gameObject.SetActive(true);    // �޸鸸 ���̰� ����

        onRevealAction = onReveal;  // ī�� ������ �� ������ �ݹ� ����

        // ī�� ��ư Ŭ�� �̺�Ʈ ������ ����
        cardButton.onClick.RemoveAllListeners();  // ���� ������ ����
        cardButton.onClick.AddListener(FlipCard); // ī�� Ŭ�� �� FlipCard �޼��� ����
    }

    // ī�带 ������ �޼���
    void FlipCard()
    {
        if (isFlipped) return;  // �̹� ī�尡 ���������� �� �̻� �������� ����
        isFlipped = true;  // ī�尡 ���������� ǥ��

        frontImage.gameObject.SetActive(true);  // �ո��� ���̰� ����
        backImage.gameObject.SetActive(false);  // �޸��� ����

        onRevealAction?.Invoke();  // ī�尡 �������� �� �ݹ� ����
    }

    // ī�尡 ���������� ���θ� ��ȯ�ϴ� �޼���
    public bool IsFlipped()
    {
        return isFlipped;  // ī�� ������ ���� ��ȯ
    }
}
