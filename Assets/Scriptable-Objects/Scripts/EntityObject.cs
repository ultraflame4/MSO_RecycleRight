using UnityEngine;

[CreateAssetMenu(fileName = "EntityObject", menuName = "ScriptableObjects/EntityObject", order = 1)]
public class EntityObject : ScriptableObject
{
    [Header("Character Information")]
    public string characterName;
    [TextArea(3, 10)]
    public string characterDesc;
    public Sprite characterSprite;
}
