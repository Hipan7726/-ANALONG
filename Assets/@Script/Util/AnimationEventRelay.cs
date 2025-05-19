using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public ZZZCharacterControllerANBI parentScript;

    public void PerformAttackHitCheck()
    {
        if (parentScript != null)
        {
            parentScript.PerformAttackHitCheck();
        }
    }

    public void Attack()
    {
        parentScript.AttackSound();
    }
}
