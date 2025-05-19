public class RuntimeCharacterData
{
    public InCharacter BaseData;
    public int CurrentHP;

    public RuntimeCharacterData(InCharacter data)
    {
        BaseData = data;
        CurrentHP = data.maxHP; // 초기값을 maxHP로 설정
    }

    public float HealthRatio => (float)CurrentHP / BaseData.maxHP;

}