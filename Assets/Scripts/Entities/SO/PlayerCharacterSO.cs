using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacterObject", menuName = "ScriptableObjects/PlayerCharacterObject", order = 1)]
public class PlayerCharacterSO : EntitySO
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float movementSpeed = 250f;

    [Header("Attack")]
    [Range(0f, 1f)] public float attackTriggerTimeFrame = 0.5f;

    [Header("Skills")]
    public float skillCooldown = 15f;
    [Range(0f, 1f)] public float skillTriggerTimeFrame = 0.5f;
    public Sprite skillIcon;

    [Header("Animation Durations")]
    public float attackDuration = 1.5f;
    public float skillDuration = 2f;
}
