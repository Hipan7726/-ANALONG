// 게임 중 캐릭터의 상태(체력 등)를 저장하는 클래스
public class InGameCharacterData
{
    // 캐릭터의 고정 데이터 (이름, 최대 체력 등), ScriptableObject로 보통 관리됨
    public InCharacter BaseData;

    // 현재 체력 - 게임 플레이 중 변화함
    public int CurrentHP;

    // 생성자 - 고정 데이터를 받아 초기화하고, 시작 체력을 최대 체력으로 설정
    public InGameCharacterData(InCharacter data)
    {
        BaseData = data;
        CurrentHP = data.maxHP;
    }

    // 현재 체력 비율을 반환 (UI 등에서 활용 가능) => 프로퍼티 문법 사용
    public float HealthRatio => (float)CurrentHP / BaseData.maxHP;
}
