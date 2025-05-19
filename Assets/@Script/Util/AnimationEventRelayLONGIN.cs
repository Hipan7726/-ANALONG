using UnityEngine;

public class AnimationEventRelayLONGIN : MonoBehaviour
{
    public ZZZCharacterControllerLONGINUS parentScript;

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
