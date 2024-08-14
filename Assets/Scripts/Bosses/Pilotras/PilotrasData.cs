using System;
using System.Collections.Generic;
using UnityEngine;
using Level.Bins;
using NPC;

namespace Bosses.Pilotras
{
    [Serializable]
    public struct PhaseObjects
    {
        public GameObject[] gameObjects;
    }

    public class PilotrasData : MonoBehaviour
    {
        #region Inspector Fields
        [Header("Scriptable Object Data")]
        [SerializeField] EntitySO data;

        [Header("Additional Data")]
        [SerializeField] float maxHealth = 500f;
        [SerializeField] float yOffset = 3f;
        [SerializeField] int numberOfPhases = 2;

        [Header("Attack Data")]
        [SerializeField] float meteorAttackDamage = 15f;
        [SerializeField] float laneAttackDamage = 25f;
        [SerializeField] float attackDelay = 2.5f;
        [SerializeField] GameObject laneAttackProjectilePrefab;
        [SerializeField] LayerMask hitMask;

        [Header("Spawnable Prefab Data")]
        [SerializeField] PhaseObjects[] spawnableNPC;
        [SerializeField] PhaseObjects[] spawnableBins;
        #endregion

        #region Public Properties
        // scriptable object data
        public EntitySO entityData => data;

        // additional data
        public float max_health => maxHealth;
        public float y_offset => yOffset;
        public int number_of_phases => numberOfPhases;

        // attack data
        public float meteor_attack_damage => meteorAttackDamage;
        public float lane_attack_damage => laneAttackDamage;
        public float attack_delay => attackDelay;
        public GameObject projectile_prefab => laneAttackProjectilePrefab;
        public LayerMask hit_mask => hitMask;

        // spawnable prefab data
        public PhaseObjects[] spawnable_npcs => spawnableNPC;
        public PhaseObjects[] spawnable_bins => spawnableBins;
        #endregion

        #region Hidden Public Variables
        [HideInInspector] public GameObject[] currentPhaseNPCs;
        [HideInInspector] public RecyclingBin[] spawnedBins;
        [HideInInspector] public FSMRecyclableNPC[] recyclables;
        [HideInInspector] public Dictionary<RecyclableType, int> npcCount = new Dictionary<RecyclableType, int>();
        [HideInInspector] public Dictionary<RecyclableType, float> binScore = new Dictionary<RecyclableType, float>();
        [HideInInspector] public Vector2 minBounds, maxBounds;
        [HideInInspector] public float damageTakenScale = 1f;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            if (spawnableNPC == null)
                Debug.LogWarning("Spawnable NPCs array is null, there are no NPCs to spawn! (PilotrasData.cs)");
            else if (spawnableNPC.Length != numberOfPhases)
                Debug.LogWarning("Spawnable NPCs array should be the length of the number of phases. (PilotrasData.cs)");
        }
        #endregion
    }
}
