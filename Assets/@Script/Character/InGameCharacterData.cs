// ���� �� ĳ������ ����(ü�� ��)�� �����ϴ� Ŭ����
public class InGameCharacterData
{
    // ĳ������ ���� ������ (�̸�, �ִ� ü�� ��), ScriptableObject�� ���� ������
    public InCharacter BaseData;

    // ���� ü�� - ���� �÷��� �� ��ȭ��
    public int CurrentHP;

    // ������ - ���� �����͸� �޾� �ʱ�ȭ�ϰ�, ���� ü���� �ִ� ü������ ����
    public InGameCharacterData(InCharacter data)
    {
        BaseData = data;
        CurrentHP = data.maxHP;
    }

    // ���� ü�� ������ ��ȯ (UI ��� Ȱ�� ����) => ������Ƽ ���� ���
    public float HealthRatio => (float)CurrentHP / BaseData.maxHP;
}
