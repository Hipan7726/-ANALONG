using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Characters/CharacterData")]
public class InCharacter : ScriptableObject
{
    public string characterName;
    public Sprite characterImage;
    public GameObject characterPrefab;
    public int maxHP;
    public int attackPower;
    public float critRate;
    public float critDamage;
}
