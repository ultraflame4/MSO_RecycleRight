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
    /// <summary>
    /// Reference to the prefab of the contaminant. Will be used to instantiate the contaminant form.
    /// </summary>
    [Tooltip("Reference to the prefab of the contaminant. Will be used to instantiate the contaminant form.")]
    public GameObject contaminantPrefab;
    /// <summary>
    /// The attack range of the contaminant. If target is within this range, the contaminant will start attacking.
    /// </summary>
    [Tooltip("The attack range of the contaminant. If target is within this range, the contaminant will start attacking.")]
    public float attackRange = 1f;
    /// <summary>
    /// The delay before each attack. in seconds.
    /// </summary>
    [Tooltip("The delay before each attack. in seconds.")]
    public float attackDelay = .1f;
    /// <summary>
    /// The damage of each attack
    /// </summary>
    [Tooltip("The damage of each attack")]
    public float attackDamage;

    /// <summary>
    /// The duration of each attack. in seconds
    /// </summary>
    [Tooltip("The duration of each attack. in seconds")]
    public float attackDuration = 1.5f;
    /// <summary>
    /// Whether the contaminant can be cleaned
    /// </summary>
    [Tooltip("Whether the contaminant can be cleaned")]
    public bool cleanable;
}

[Serializable]
public class RecyclableConfig
{
    /// <summary>
    /// The type of recyclable
    /// </summary>
    [Tooltip("The type of recyclable")]
    public RecyclableType recyclableType;
    /// <summary>
    /// Reference to the prefab of the recyclable. Will be used to instantiate the recyclable.
    /// </summary>
    [Tooltip("Reference to the prefab of the recyclable. Will be used to instantiate the recyclable.")]
    public GameObject recyclablePrefab;
}

[Serializable]
public class CommonConfig
{
    [Tooltip("Max health for this contaminant / recyclable.")]
    public float maxHealth = 100f;
    [Tooltip("Movement speed for this contaminant / recyclable.")]
    public float movementSpeed = 250f;
    [Tooltip("How far this contaminant / recyclable can see. (Detection range)")]
    public float sightRange = 3f;
}

[CreateAssetMenu(fileName = "NPC", menuName = "ScriptableObjects/TrashNpc", order = 1)]
public class TrashNpcSO : EntitySO
{

    /// <summary>
    /// Base NPC type.
    /// </summary>
    [Tooltip("Base NPC type.")]
    public TrashNPCType trashNPCType;
    /// <summary>
    /// Common config for both contaminant and recyclable.
    /// </summary>
    [Tooltip("Common config for both contaminant and recyclable.")]
    public CommonConfig common;

    /// <summary>
    /// Recyclable Specific Configuration
    /// </summary>
    [Tooltip("Recyclable Specific Configuration")]
    public RecyclableConfig recyclableConfig;

    /// <summary>
    /// Contaminant Specific Configuration
    /// </summary>
    [Tooltip("Contaminant Specific Configuration")]
    public ContaminantConfig contaminantConfig;

}
