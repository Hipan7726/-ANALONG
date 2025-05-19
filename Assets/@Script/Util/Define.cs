using UnityEngine;

public class Define : MonoBehaviour
{
    // Unity�� Ű ���� �̸� ������ ���̴�. ���� ���� ����ϸ� "Mouse X", "Horizontal" ���� ���ڿ��� ���� �Է��ϴ�
    // �ͺ��� �������� ���� ������ ���������ϴ�.
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

    // ���Ϳ� �߰�
    public readonly static int isMoving = Animator.StringToHash("isMoving");
    public readonly static int Attack = Animator.StringToHash("Attack");
    #endregion

    #region Tags
    public const string Player = "Player";
    public const string Enemy = "Enemy";
    #endregion
}
