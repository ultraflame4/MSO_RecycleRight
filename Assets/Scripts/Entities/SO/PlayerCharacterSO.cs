using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacterObject", menuName = "ScriptableObjects/PlayerCharacterObject", order = 1)]
public class PlayerCharacterSO : EntitySO
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float movementSpeed = 250f;

    [Header("Attack")]
    [Range(0f, 1f)]
    public float attackTriggerTimeFrame = 0.5f;

    [Header("Skills")]
    public float skillCooldown = 15f;
    [Range(0f, 1f)]
    public float skillTriggerTimeFrame = 0.5f;
    public Sprite skillIcon;

    [Header("Animation Durations")]
    public float attackDuration = 1.5f;
    public float skillDuration = 2f;
    public float deathDuration = 3.5f;

    [Header("UI")]
    public Sprite characterSelectionSprite;

    [Header("Character Tags")]
    public CharacterAttackType attackType;
    public CharacterAttackTarget attackTarget;
    public CharacterRoles role1 = CharacterRoles.NONE;
    public CharacterRoles role2 = CharacterRoles.NONE;
    public CharacterRoles role3 = CharacterRoles.NONE;

    [Header("Prefab")]
    public GameObject prefab;
}

#region Character Tag Enums
public enum CharacterAttackType
{
    MELEE, 
    RANGE
}

public enum CharacterAttackTarget
{
    SINGLE_TARGET, 
    MULTI_TARGET
}

public enum CharacterRoles
{
    NONE, 
    DAMAGE, 
    PUSHER, 
    CLEANER, 
    CROWD_CONTROL, 
    SUPPORT, 
    HEALER, 
}
#endregion
