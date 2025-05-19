using UnityEngine;

public class AnimationEventRelayANBI : MonoBehaviour
{
    public ZZZCharacterControllerANBI parentScript;

    public void PerformAttackHitCheck()
    {
        if (parentScript != null)
        {
            parentScript.PerformAttackHitCheck();
        }
    }
}
