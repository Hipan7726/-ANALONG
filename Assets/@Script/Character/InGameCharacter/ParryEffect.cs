using UnityEngine;

public class ParryEffect : MonoBehaviour
{
    public static ParryEffect Instance; // �̱��� �ν��Ͻ�

    public GameObject ParryEffectPrefab; // �и� ����Ʈ ������
    private GameObject _currentEffect; // ���� ������ ����Ʈ ������Ʈ

    private void Awake()
    {
        // �̱��� ���� - �̹� �����ϸ� �ı�
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowParryEffect(Vector3 position)
    {
        if (ParryEffectPrefab == null)
        {
            Debug.LogWarning("ParryEffectPrefab is not assigned!"); // ������ ���� ���
            return;
        }

        // �÷��̾� ��ġ �������� �Ӹ� ���� ����Ʈ ����
        Vector3 effectPos = position + Vector3.up * 2f;

        // ���� ����Ʈ�� �����ϸ� ����
        if (_currentEffect != null)
        {
            Destroy(_currentEffect);
            _currentEffect = null;
        }

        // ���ο� ����Ʈ ����
        _currentEffect = Instantiate(ParryEffectPrefab, effectPos, Quaternion.identity);
    }

    public void DestroyEffect()
    {
        // �ܺο��� �������� ����Ʈ ������ �� ���
        Destroy(_currentEffect);
    }
}
