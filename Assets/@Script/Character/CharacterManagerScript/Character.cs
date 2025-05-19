using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character/Create New Character")]
public class CharacterDataSO : ScriptableObject
{
    public string characterID;
    public string characterName;
    public Sprite characterImage;
    public GameObject characterPrefab;
    public int hp;
    public int damage;
    public int penetratingpower;    //관통력
    public int criticalHit;         //치명타 확률
    public int criticalHitDamage;   //치명타 피해

    public AudioClip voiceClip;      //음성 클립

}
