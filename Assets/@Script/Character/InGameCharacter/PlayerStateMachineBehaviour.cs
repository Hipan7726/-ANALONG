using UnityEngine;

//�� Ŭ������ Unity�� StateMachineBehaviour�� ��ӹ޾� �ִϸ��̼� ����(State) ����� ������ ó���ϴ� Ŭ����
//�ִϸ��̼��� �������϶� ������ �Լ�
public class PlayerStateMachineBehaviour : StateMachineBehaviour//�ִϸ��̼� ����(State)�� ���õ� �̺�Ʈ�� ó���ϴ� Ư���� Ŭ����
{
    //�ִ� ���̼��� ����Ǵ� ���� ������ ���� ȣ��Ǵ� �Լ�
    //���� �ִϸ��̼� ����ð��� �̿��Ͽ� �޺� ������ ���� ���θ� �Ǵ� �Ѵ�.
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        float currentTime = stateInfo.normalizedTime;
        bool isNextCombo = animator.GetBool(Define.isNextCombo);

        if (currentTime < 0.9f && currentTime > 0.7f && isNextCombo)
        {
            int comboCount = animator.GetInteger(Define.ComboCount);
            comboCount = comboCount < 2 ? ++comboCount : 0;
            animator.SetInteger(Define.ComboCount, comboCount);
        }

        if (currentTime >= 1f)
        {
            animator.SetInteger(Define.ComboCount, 0);
            animator.SetBool(Define.isAttacking, false);  // ���� ���� ����
            animator.SetBool(Define.isNextCombo, false);

            // ������ ������ �̵��� ����մϴ�.
            ZZZCharacterControllerANBI characterController = animator.GetComponent<ZZZCharacterControllerANBI>();
            if (characterController != null)
            {
                characterController.IsAttacking = false;  // ���� ���� ����
            }
        }
    }

    //�ִϸ��̼� ����(State)�� ���۵� �� ȣ��Ǵ� �Լ� �Դϴ�.
    //�޺� �Է� ����(isNextcombo)�� �ʱ�ȭ �մϴ�.
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(Define.isNextCombo, false);
    }
}
