using UnityEngine;

namespace Bosses.Pilotras
{
    public class PilotrasBehaviourData : MonoBehaviour
    {
        #region Inspector Fields
        [Header("Placing State")]
        [SerializeField] Vector2 placingDuration;
        [SerializeField] Vector2 placingCooldown;
        [SerializeField] Vector2 placeNPCAmount;
        [SerializeField] float placingSpeed = 0.5f;

        [Header("Bin Drop State")]
        [SerializeField] Vector2 binDropDuration;
        [SerializeField] Vector2 binDropCooldown;
        [SerializeField] float binDropSpeed = 1f;
        [SerializeField] float binDropForce = 25f;
        [SerializeField] float scoredDamage = 100f;
        [SerializeField] float contaminatedHeal = 50f;
        [SerializeField] LayerMask dropDetectionMask;

        [Header("Bin Positioning")]
        [SerializeField] float binSpacing = 3f;
        [SerializeField] Vector2 binPositionOffset;
        [SerializeField] Transform activeBins, inactiveBins;
        #endregion

        #region Public Properties
        // placing state
        public float placing_duration => Random.Range(placingDuration.x, placingDuration.y);
        public float placing_cooldown => Random.Range(placingCooldown.x, placingCooldown.y);
        public int place_npc_amount => Mathf.RoundToInt(Random.Range(placeNPCAmount.x, placeNPCAmount.y));
        public float placing_speed => placingSpeed;

        // bin drop state
        public float bin_drop_duration => Random.Range(binDropDuration.x, binDropDuration.y);
        public float bin_drop_cooldown => Random.Range(binDropCooldown.x, binDropCooldown.y);
        public float bin_drop_speed => binDropSpeed;
        public float bin_drop_force => binDropForce;
        public float scored_damage => scoredDamage;
        public float contaminated_heal => contaminatedHeal;
        public LayerMask drop_detection_mask => dropDetectionMask;

        // bin positioning
        public float bin_spacing => binSpacing;
        public Vector2 bin_offset => binPositionOffset;
        public Transform active_bins => activeBins;
        public Transform inactive_bins => inactiveBins;
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            // ensure place npc amount is an integer
            placeNPCAmount = new Vector2(Mathf.RoundToInt(placeNPCAmount.x), Mathf.RoundToInt(placeNPCAmount.y));
        }
        #endregion
    }
}
