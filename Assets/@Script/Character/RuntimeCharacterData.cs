public class RuntimeCharacterData
{
    public InCharacter BaseData;
    public int CurrentHP;

    public RuntimeCharacterData(InCharacter data)
    {
        BaseData = data;
        CurrentHP = data.maxHP; // �ʱⰪ�� maxHP�� ����
    }

    public float HealthRatio => (float)CurrentHP / BaseData.maxHP;

}