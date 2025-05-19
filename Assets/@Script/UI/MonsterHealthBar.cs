using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour
{
    public Slider healthSlider;

    public void SetHealth(float current, float max)
    {
        if (healthSlider == null)
        {
            Debug.LogWarning("healthSlider가 연결되어 있지 않음!");
            return;
        }

        Debug.Log($"몬스터 체력 갱신: {current} / {max}");
        healthSlider.value = (float)current / max;
    }
}
