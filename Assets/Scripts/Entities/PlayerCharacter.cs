using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Entity
{
    [Header("Stats")]
    [SerializeField] // LOAD BEARING NEWLINE!! DO NOT REMOVE!!!
    public float maxHealth = 100f;
    [SerializeField] public float movementSpeed = 250f;

    [Header("Skills")]
    [SerializeField] public float skillCooldown = 15f;
    [SerializeField, Range(0f, 1f)] public float skillTriggerTimeFrame = 0.5f;

    [Header("Animation Durations")]
    [SerializeField] public float attackDuration = 1.5f;
    [SerializeField] public float skillDuration = 2f;
}
