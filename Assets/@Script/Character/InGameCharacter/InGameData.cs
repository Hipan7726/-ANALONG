using UnityEngine;

[System.Serializable]
public class CharacterSlot
{
    public InCharacter characterData;
    [HideInInspector] public GameObject characterObject;
    [HideInInspector] public ZZZCharacterControllerANBI controllerANBI;
    [HideInInspector] public ZZZCharacterControllerLONGINUS controllerLonginus;
}
