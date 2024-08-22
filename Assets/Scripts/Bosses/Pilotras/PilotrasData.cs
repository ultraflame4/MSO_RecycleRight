using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] int maxNPCCount = 10;

        [Header("Attack Data")]
        [SerializeField] float meteorAttackDamage = 15f;
        [SerializeField] float laneAttackDamage = 25f;
        [SerializeField] float binDropDamage = 25f;
        [SerializeField] float binDropPlayerStunDuration = 1f;
        [SerializeField] float attackDelay = 2.5f;
        [SerializeField] GameObject laneAttackProjectilePrefab;
        [SerializeField] LayerMask hitMask;

        [Header("Sound Effect")]
        [SerializeField] AudioClip[] roar;
        [SerializeField] AudioClip binDrop, meteorAttack;

        [Header("Health Bar")]
        [SerializeField] Slider healthbar;
        [SerializeField] Transform phaseIndicatorParent;
        [SerializeField] GameObject phaseIndicatorPrefab;

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
        public int max_npc_count => maxNPCCount;

        // attack data
        public float meteor_attack_damage => meteorAttackDamage;
        public float lane_attack_damage => laneAttackDamage;
        public float bin_drop_damage => binDropDamage;
        public float bin_drop_player_stun_duration => binDropPlayerStunDuration;
        public float attack_delay => attackDelay;
        public GameObject projectile_prefab => laneAttackProjectilePrefab;
        public LayerMask hit_mask => hitMask;

        // sound effects
        public AudioClip[] sfx_roar => roar;
        public AudioClip sfx_bin_drop => binDrop;
        public AudioClip sfx_meteor_attack => meteorAttack;

        // health bar
        public Slider health_bar => healthbar;
        public Transform phase_indicator_parent => phaseIndicatorParent;
        public GameObject phase_indicator_prefab => phaseIndicatorPrefab;

        // spawnable prefab data
        public PhaseObjects[] spawnable_npcs => spawnableNPC;
        public PhaseObjects[] spawnable_bins => spawnableBins;
        #endregion

        #region Hidden Public Variables
        [HideInInspector] public GameObject[] currentPhaseNPCs;
        [HideInInspector] public GameObject[] phaseIndicators;
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
