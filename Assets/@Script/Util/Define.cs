using UnityEngine;

public class Define : MonoBehaviour
{
    // Unity의 키 값을 미리 정의한 것이다. 위와 같이 사용하면 "Mouse X", "Horizontal" 같은 문자열을 직접 입력하는
    // 것보다 가독성과 유지 보수가 용이해집니다.
    #region Input
    public const string MouseX = "Mouse X";
    public const string MouseY = "Mouse Y";
    public const string MouseScroll = "Mouse ScrollWheel";
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";
    #endregion

    #region Animator Parameters
    public readonly static int Speed = Animator.StringToHash("Speed");
    public readonly static int Run = Animator.StringToHash("Run");
    public readonly static int Rest = Animator.StringToHash("Rest");
    public readonly static int ComboCount = Animator.StringToHash("ComboCount");
    public readonly static int isNextCombo = Animator.StringToHash("isNextCombo");
    public readonly static int isAttacking = Animator.StringToHash("isAttacking");
    public readonly static int Hit = Animator.StringToHash("Hit");
    public readonly static int Padding = Animator.StringToHash("Padding");
    public readonly static int Dead = Animator.StringToHash("Dead");

    // 몬스터용 추가
    public readonly static int isMoving = Animator.StringToHash("isMoving");
    public readonly static int Attack = Animator.StringToHash("Attack");
    #endregion

    #region Tags
    public const string Player = "Player";
    public const string Enemy = "Enemy";
    #endregion
}
