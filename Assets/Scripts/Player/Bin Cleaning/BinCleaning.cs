using System;
using System.Linq;
using UnityEngine;
using Patterns.FSM;
using Level;
using Level.Bins;
using Entity.Data;
using Player.BinCleaning.FSM;

namespace Player.BinCleaning
{
    public class BinCleaning : StateMachine<BinCleaning>
    {
        #region Inspector Fields
        [Tooltip("Duration to clean bin")]
        public float binCleanDuration = 15f;
        [Tooltip("Maximum distance from bin to be able to clean it")]
        public float binCleanRange = 2f;
        [Tooltip("Offset character from bin when cleaning it")]
        public Vector3 binCleanOffset;
        [SerializeField, Tooltip("Layer mask to detect bin")]
        private LayerMask binMask;
        #endregion

        #region FSM States
        public DefaultState Default { get; private set; }
        public MovingState Moving { get; private set; }
        public CleaningState Cleaning { get; private set; }
        #endregion

        #region Public Properties
        public PlayerController controller { get; private set; }
        public PlayerCharacter currentCharacterData { get; private set; }
        #endregion

        #region MonoBehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {
            // get reference to player controller
            controller = GetComponentInParent<PlayerController>();
            // get reference to the character data of this character
            currentCharacterData = GetComponent<PlayerCharacter>();
            // subscribe to character change event
            controller.CharacterManager.CharacterChanged += OnCharacterChange;
            // subscribe to zone change event
            if (controller.LevelManager != null)
                controller.LevelManager.ZoneChanged += OnZoneChange;
            
            // handle fsm
            // initialize states
            Default = new DefaultState(this, this);
            Moving = new MovingState(this, this);
            Cleaning = new CleaningState(this, this);
            // initialize fsm
            Initialize(Default);
        }
        #endregion

        #region Cleaning Behaviour Methods
        public void SetCleaning(bool cleaning, Transform parent)
        {
            // set cleaning state of character
            currentCharacterData.IsCleaning = cleaning;
            // set spawn, disable collider to prevent taking damage when cleaning
            currentCharacterData.SetSpawn(cleaning);
            currentCharacterData.collider.enabled = false;
            // set character parent
            currentCharacterData.transform.parent = parent;
            // set character local position
            currentCharacterData.transform.localPosition = Vector2.zero;
            // offset position if at bin
            if (!cleaning) return;
            currentCharacterData.transform.localPosition = 
                currentCharacterData.transform.localPosition += binCleanOffset;
        }

        void CleanBin(RecyclingBin bin)
        {
            // set bin state to start cleaning
            bin.SetCleaning();
            // switch to next character in the array
            // get index of current character
            int index = Array.IndexOf(controller.CharacterManager.character_instances, currentCharacterData);
            // increment index by 1, if last item, reset index to 0
            index = index < (controller.CharacterManager.character_instances.Length - 1) && 
                controller.CharacterManager.character_instances.Length > 1 ? index++ : 0;
            // switch to next character in array
            controller.CharacterManager.SwitchCharacter(index);
            // setup cleaning
            SetCleaning(true, bin.transform);
            // switch state to cleaning state
            SwitchState(Cleaning);
        }
        #endregion

        #region Event Listeners
        void OnCharacterChange(PlayerCharacter data)
        {
            // ensure switching to current character
            if (data != currentCharacterData) return;
            // check if near a bin
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, binCleanRange, binMask);
            // filter out uncontaminated bins
            RecyclingBin[] bins = hits
                .Select(x => x.GetComponent<RecyclingBin>())
                .Where(x => x != null && (x.binState == BinState.CONTAMINATED || x.binState == BinState.INFESTED))
                .OrderBy(x => Vector3.Distance(x.transform.position, transform.position))
                .ToArray();
            // check if anything still remains after the filter
            if (bins.Length <= 0) return;
            // handle starting bin cleaning
            CleanBin(bins[0]);
        }

        void OnZoneChange(LevelZone zone)
        {
            // force switch to moving state, run towards player's current position and reset
            SwitchState(Moving);
        }
        #endregion

        #region Gizmos
        void OnDrawGizmosSelected()
        {
            // ensure player controller is not null
            if (controller == null) return;
            // show bin interaction range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, binCleanRange);
        }
        #endregion
    }   
}
