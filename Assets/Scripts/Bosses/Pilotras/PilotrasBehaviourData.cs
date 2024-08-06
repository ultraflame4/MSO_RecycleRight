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
        #endregion

        #region Public Properties
        // placing state
        public float placing_duration => Random.Range(placingDuration.x, placingDuration.y);
        public float placing_cooldown => Random.Range(placingCooldown.x, placingCooldown.y);
        public int place_npc_amount => Mathf.RoundToInt(Random.Range(placeNPCAmount.x, placeNPCAmount.y));
        public float placing_speed => placingSpeed;
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
