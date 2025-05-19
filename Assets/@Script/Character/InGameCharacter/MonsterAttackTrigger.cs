using UnityEngine;

public class MonsterAttackTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var anbi = other.GetComponent<ZZZCharacterControllerANBI>();
        if (anbi != null)
        {
            anbi.TakeHit(MonsterController.Instance.AttackPower);
            return;
        }

        var longinus = other.GetComponent<ZZZCharacterControllerLONGINUS>();
        if (longinus != null)
        {
            longinus.TakeHit(MonsterController.Instance.AttackPower);
        }
    }
}
