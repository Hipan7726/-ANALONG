using UnityEngine;
using UnityEngine.UI;

public class GachaCard : MonoBehaviour
{
    public Image frontImage;  // 카드 앞면 이미지
    public Image backImage;   // 카드 뒷면 이미지
    public Button cardButton; // 카드 클릭 버튼

    private bool isFlipped = false;  // 카드가 뒤집혔는지 여부
    private System.Action onRevealAction;  // 카드 뒤집을 때 실행될 콜백

    // 카드의 앞면과 뒷면 이미지를 설정하고, 카드 뒤집을 때 실행할 콜백 설정
    public void SetCard(Sprite frontSprite, Sprite backSprite, System.Action onReveal)
    {
        frontImage.sprite = frontSprite;  // 카드 앞면 스프라이트 설정
        backImage.sprite = backSprite;    // 카드 뒷면 스프라이트 설정

        frontImage.gameObject.SetActive(false);  // 처음에는 앞면을 보이지 않도록 설정
        backImage.gameObject.SetActive(true);    // 뒷면만 보이게 설정

        onRevealAction = onReveal;  // 카드 뒤집을 때 실행할 콜백 설정

        // 카드 버튼 클릭 이벤트 리스너 설정
        cardButton.onClick.RemoveAllListeners();  // 기존 리스너 제거
        cardButton.onClick.AddListener(FlipCard); // 카드 클릭 시 FlipCard 메서드 실행
    }

    // 카드를 뒤집는 메서드
    void FlipCard()
    {
        if (isFlipped) return;  // 이미 카드가 뒤집혔으면 더 이상 실행하지 않음
        isFlipped = true;  // 카드가 뒤집혔음을 표시

        frontImage.gameObject.SetActive(true);  // 앞면을 보이게 설정
        backImage.gameObject.SetActive(false);  // 뒷면은 숨김

        onRevealAction?.Invoke();  // 카드가 뒤집혔을 때 콜백 실행
    }

    // 카드가 뒤집혔는지 여부를 반환하는 메서드
    public bool IsFlipped()
    {
        return isFlipped;  // 카드 뒤집힘 상태 반환
    }
}
