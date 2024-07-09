using System;
using Level.Bins;
using UnityEngine;

public enum TrashNPCType
{
    /// <summary>
    /// Will contaminate bins
    /// </summary>
    OnlyContaminant,
    /// <summary>
    /// Can be recycled, can also be contaminated
    /// </summary>
    Recyclable,
}

[Serializable]
public class ContaminantConfig
{
    public GameObject contaminantPrefab;
    [Tooltip("The attack range of the contaminant. If target is within this range, the contaminant will start attacking.")]
    public float attackRange = 1f;
    [Tooltip("The delay before each attack. in seconds")]
    public float attackDelay = .1f;
    [Tooltip("The damage of each attack")]
    public float attackDamage;
    [Tooltip("Whether the contaminant can be cleaned")]
    public bool cleanable;
}

[Serializable]
public class RecyclableConfig
{
    public RecyclableType recyclableType;
    public GameObject recyclablePrefab;
}
[CreateAssetMenu(fileName = "NPC", menuName = "ScriptableObjects/TrashNpc", order = 1)]
public class TrashNpcSO : EntitySO
{
    [Header("Base Stats")]
    [Tooltip("Max health for this contaminant / recyclable.")]
    public float maxHealth = 100f;
    [Tooltip("Movement speed for this contaminant / recyclable.")]
    public float movementSpeed = 250f;
    [Tooltip("How far this contaminant / recyclable can see. (Detection range)")]
    public float sightRange = 3f;

    [Tooltip("Base NPC type.")]
    public TrashNPCType trashNPCType;
    
    public RecyclableConfig recyclableConfig;

    public ContaminantConfig contaminantConfig;

}
