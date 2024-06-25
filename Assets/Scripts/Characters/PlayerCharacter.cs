using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    [field: Header("Stats")]
    [field: SerializeField] public float maxHealth { get; private set; } = 100f;
    [field: SerializeField] public float movementSpeed { get; private set; } = 250f;
    [field: SerializeField] public float attackDamage { get; private set; } = 25f;

    [field: Header("Skills")]
    [field: SerializeField] public float skillCooldown { get; private set; } = 15f;

    [field: Header("Animation Durations")]
    [field: SerializeField] public float attackDuration { get; private set; } = 1.5f;
    [field: SerializeField] public float skillDuration { get; private set; } = 2f;

    // methods to be overrided depending on the character
    public virtual void TriggerAttack() {}
    public virtual void TriggerPassive() {}
    public virtual void TriggerSkill() {}
}
