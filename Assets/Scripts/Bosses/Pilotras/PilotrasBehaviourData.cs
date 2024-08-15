using UnityEngine;

namespace Bosses.Pilotras
{
    public class PilotrasBehaviourData : MonoBehaviour
    {
        #region Inspector Fields
        [Header("Start State")]
        [SerializeField] float spawnDuration = 2.5f;
        
        [Header("Placing State")]
        [SerializeField] Vector2 placingDuration;
        [SerializeField] Vector2 placingCooldown;
        [SerializeField] Vector2Int placeNPCAmount;
        [SerializeField] float placingSpeed = 0.5f;
        [SerializeField] float placingDelay = 0.5f;

        [Header("Meteor Shower Attack State")]
        [SerializeField] Vector2 meteorAttackDuration;
        [SerializeField] Vector2Int attackPlaceNPCAmount;
        [SerializeField, Range(0f, 1f)] float baseEnterAttackChance = 0.15f;
        [SerializeField, Range(0f, 1f)] float attackChanceIncreasePerPhase = 0.3f;
        [SerializeField] float dropAttackRange = 3f;
        [SerializeField] float dropSpeed = 0.25f;

        [Header("Bin Drop State")]
        [SerializeField] Vector2 binDropDuration;
        [SerializeField] Vector2 binDropCooldown;
        [SerializeField] float binDropSpeed = 1f;
        [SerializeField] float binDropDelay = 0.5f;
        [SerializeField] float binDropForce = 25f;
        [SerializeField] float postDropStunDuration = 5f;
        [SerializeField] float scoredDamage = 100f;
        [SerializeField] float contaminatedHeal = 50f;
        [SerializeField] LayerMask dropDetectionMask;

        [Header("Bin Positioning")]
        [SerializeField] float binSpacing = 3f;
        [SerializeField] Vector2 binPositionOffset;

        [Header("Topple State")]
        [SerializeField] float toppleThreshold = 5f;
        [SerializeField] float toppleDuration = 8f;
        [SerializeField] float toppleDamageMultiplier = 7f;
        [SerializeField] Vector2 colliderOffset;

        [Header("Phase Change State")]
        [SerializeField] float phaseChangeDuration = 5f;

        [Header("Lane Attack State")]
        [SerializeField] float laneAttackDuration = 1f;
        [SerializeField] Vector2 laneAttackCooldown;
        #endregion

        #region Public Properties
        // start state
        public float spawn_duration => spawnDuration;
        
        // placing state
        public float placing_duration => Random.Range(placingDuration.x, placingDuration.y);
        public float placing_cooldown => Random.Range(placingCooldown.x, placingCooldown.y);
        public int place_npc_amount => Random.Range(placeNPCAmount.x, placeNPCAmount.y);
        public float placing_speed => placingSpeed;
        public float placing_delay => placingDelay;

        // meteor shower attack state
        public float meteor_attack_duration => Random.Range(meteorAttackDuration.x, meteorAttackDuration.y);
        public int meteor_attack_place_npc =>  Random.Range(attackPlaceNPCAmount.x, attackPlaceNPCAmount.y);
        public float base_enter_attack_chance => baseEnterAttackChance;
        public float attack_chance_increase => attackChanceIncreasePerPhase;
        public float drop_attack_range => dropAttackRange;
        public float drop_speed => dropSpeed;

        // bin drop state
        public float bin_drop_duration => Random.Range(binDropDuration.x, binDropDuration.y);
        public float bin_drop_cooldown => Random.Range(binDropCooldown.x, binDropCooldown.y);
        public float bin_drop_speed => binDropSpeed;
        public float bin_drop_delay => binDropDelay;
        public float bin_drop_force => binDropForce;
        public float post_drop_stun_duration => postDropStunDuration;
        public float scored_damage => scoredDamage;
        public float contaminated_heal => contaminatedHeal;
        public LayerMask drop_detection_mask => dropDetectionMask;

        // bin positioning
        public float bin_spacing => binSpacing;
        public Vector2 bin_offset => binPositionOffset;

        // topple state
        public float topple_threshold => toppleThreshold;
        public float topple_duration => toppleDuration;
        public float topple_damage_multiplier => toppleDamageMultiplier;
        public Vector2 collider_offset => colliderOffset;

        // phase change state
        public float phase_change_duration => phaseChangeDuration;

        // lane attack state
        public float lane_attack_duration => laneAttackDuration;
        public float lane_attack_cooldown => Random.Range(laneAttackCooldown.x, laneAttackCooldown.y);
        #endregion
    }
}
