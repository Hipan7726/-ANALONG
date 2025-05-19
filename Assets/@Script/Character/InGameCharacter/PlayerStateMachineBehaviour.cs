using UnityEngine;

//이 클래스는 Unity의 StateMachineBehaviour를 상속받아 애니메이션 상태(State) 변경과 로직을 처리하는 클래스
//애니메이션이 실행중일때 나오는 함수
public class PlayerStateMachineBehaviour : StateMachineBehaviour//애니메이션 상태(State)와 관련된 이벤트를 처리하는 특수한 클래스
{
    //애니 메이션이 실행되는 동안 프레임 마다 호출되는 함수
    //현재 애니메이션 진행시간을 이용하여 콤보 공격의 진행 여부를 판단 한다.
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
            animator.SetBool(Define.isAttacking, false);  // 공격 상태 해제
            animator.SetBool(Define.isNextCombo, false);

            // 공격이 끝나면 이동을 허용합니다.
            ZZZCharacterControllerANBI characterController = animator.GetComponent<ZZZCharacterControllerANBI>();
            if (characterController != null)
            {
                characterController.IsAttacking = false;  // 공격 상태 해제
            }
        }
    }

    //애니메이션 상태(State)가 시작될 때 호출되는 함수 입니다.
    //콤보 입력 상태(isNextcombo)를 초기화 합니다.
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(Define.isNextCombo, false);
    }
}
