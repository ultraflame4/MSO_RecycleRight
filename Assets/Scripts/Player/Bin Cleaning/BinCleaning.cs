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
        [Header("Bin Cleaning")]
        [Tooltip("Duration to clean bin")]
        public float binCleanDuration = 15f;
        [Tooltip("Maximum distance from bin to be able to clean it")]
        public float binCleanRange = 2f;
        [Tooltip("Offset character from bin when cleaning it")]
        public Vector3 binCleanOffset;
        [SerializeField, Tooltip("Layer mask to detect bin")]
        private LayerMask binMask;

        [Header("Movement")]
        [Tooltip("Speed to return to player after cleaning bin")]
        public float returnSpeed = 10f;
        [Tooltip("Distance from player threshold to count as having reached the target return location")]
        public float returnThreshold = 2f;
        #endregion

        #region FSM States
        public DefaultState Default { get; private set; }
        public MovingState Moving { get; private set; }
        public CleaningState Cleaning { get; private set; }
        #endregion

        #region Public Properties
        public PlayerController controller { get; private set; }
        public PlayerCharacter currentCharacterData { get; private set; }
        public RecyclingBin cleaningBin { get; private set; }
        public Animator anim { get; private set; }
        #endregion

        #region Hidden Variables
        private PlayerCharacter activeCharacterData;
        [HideInInspector] public bool lastCharacter = false;
        #endregion

        #region MonoBehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {
            // get reference to player controller
            controller = GetComponentInParent<PlayerController>();
            // get reference to the character data of this character
            currentCharacterData = GetComponent<PlayerCharacter>();
            // get reference to animator
            anim = GetComponentInChildren<Animator>();
            // subscribe to character change event
            controller.CharacterManager.CharacterChanged += OnCharacterChange;
            // subscribe to zone change event
            if (LevelManager._instance != null)
                LevelManager._instance.ZoneChanged += OnZoneChange;
            
            // handle fsm
            // initialize states
            Default = new DefaultState(this, this);
            Moving = new MovingState(this, this);
            Cleaning = new CleaningState(this, this);
            // initialize fsm
            Initialize(Default);
        }

        new void Update()
        {
            // call update in base class
            base.Update();
            // handle last character
            if (!lastCharacter) return;
            if (!AvailableCleaningCharacter(out PlayerCharacter availableCharacter)) return;
            // once found available character, enable controller, and switch to that character
            lastCharacter = false;
            availableCharacter.OverrideSwitchable = false;
            controller.gameObject.SetActive(true);
            controller.transform.position = availableCharacter.transform.position;
            controller.CharacterManager.SwitchCharacter(Array.IndexOf(controller.CharacterManager.character_instances, availableCharacter));
        }
        #endregion

        #region Cleaning Behaviour Methods
        public void SetCleaning(bool cleaning, Transform parent)
        {   
            // check if character is enabled
            if (!currentCharacterData.Enabled) currentCharacterData.renderer.enabled = cleaning;
            // set renderer, disable collider to prevent taking damage when cleaning
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

        void CleanBin()
        {
            // set bin state to start cleaning
            cleaningBin.SetCleaning();
            // set cleaning state of character
            currentCharacterData.IsCleaning = true;
            // switch back to original character
            int index = Array.IndexOf(controller.CharacterManager.character_instances, activeCharacterData);
            // if original character is self, do not switch back, instead increment and switch to next character
            if (activeCharacterData == currentCharacterData)
            {
                // search for available character to clean bin
                for (int i = 0; i < controller.CharacterManager.character_instances.Length; i++)
                {
                    if (controller.CharacterManager.character_instances[i].IsCleaning) continue;
                    index = i;
                    break;
                }
            }
            // check if an index is found
            if (index < 0) return;
            // switch to next character in array
            controller.CharacterManager.SwitchCharacter(index);
            // disable current character
            currentCharacterData.SetSpawn(false);
            // switch state to cleaning state
            SwitchState(Cleaning);
            // check if is last available character to clean
            lastCharacter = !AvailableCleaningCharacter(out PlayerCharacter availableCharacter);
            // disable controller if is last character
            controller.gameObject.SetActive(!lastCharacter);
        }

        bool AvailableCleaningCharacter(out PlayerCharacter availableCharacter)
        {
            // set default output to null
            availableCharacter = null;
            // loop through all characters to search for a character that be switched into
            foreach (PlayerCharacter character in controller.CharacterManager.character_instances)
            {
                if (character.IsCleaning || character.Health <= 0) continue;
                availableCharacter = character;
                return true;
            }
            return false;
        }
        #endregion

        #region Event Listeners
        void OnCharacterChange(PlayerCharacter prev, PlayerCharacter curr)
        {
            // cache previous character if it is not null
            activeCharacterData = prev == null ? curr : prev;
            // ensure switch is not due to death state
            if (controller.current_state_name == "Player.FSM.PlayerDeathState") return;
            // ensure switching to current character
            if (curr != currentCharacterData) return;
            // do not run if character is cleaning
            if (currentCharacterData.IsCleaning) return;
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
            // ensure bin is cleanable before starting cleaning proccess
            if (!bins[0].cleanable) return;
            // set cleaning bin
            cleaningBin = bins[0];
            // handle starting bin cleaning
            CleanBin();
        }

        void OnZoneChange(LevelZone zone)
        {
            // do not run if it is first zone
            if (zone == LevelManager.Instance.zones[0]) return;
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
            // show return threshold range
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, returnThreshold);
        }
        #endregion
    }   
}
