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
    public int penetratingpower;    //�����
    public int criticalHit;         //ġ��Ÿ Ȯ��
    public int criticalHitDamage;   //ġ��Ÿ ����

    public AudioClip voiceClip;      //���� Ŭ��

}
