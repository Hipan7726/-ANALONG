using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour
{
    public Slider healthSlider;

    public void SetHealth(float current, float max)
    {
        if (healthSlider == null)
        {
            Debug.LogWarning("healthSlider�� ����Ǿ� ���� ����!");
            return;
        }

        Debug.Log($"���� ü�� ����: {current} / {max}");
        healthSlider.value = (float)current / max;
    }
}
