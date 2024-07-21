using System;
using UnityEngine;
using Patterns.FSM;
using Entity.Data;
using Player.FSM;
using Behaviour = Player.Behaviours.Behaviour;
using Level;

namespace Player
{
    public class PlayerController : StateMachine<PlayerController>
    {
        #region Inspector Fields
        [SerializeField] private CharacterManager characterManager;
        #endregion

        #region States
        public PlayerDefaultState DefaultState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }
        public PlayerSkillState SkillState { get; private set; }
        public PlayerMoveToZoneState MoveToZoneState { get; private set; }
        public PlayerDeathState DeathState { get; private set; }
        #endregion

        #region Other Properties
        public PlayerCharacter Data { get; private set; }
        public Behaviour CharacterBehaviour { get; private set; }
        public DirectionPointer PointerManager { get; private set; }
        private Animator _anim;
        // Explicitly return null if _anim is equals null (If _anim == null, it may not be the real null, Unity overrides the equality operator to make some stuff equal to null (destroyed objects, missing components, etc))
        public Animator anim => _anim == null ? null : _anim;
        public CharacterManager CharacterManager => characterManager;
        public Transform pointer => transform.GetChild(0);

        
        private static PlayerController _instance;
        public static PlayerController Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new NullReferenceException("There is no player instance in the scene!");
                }
                return _instance;
            }
        }
        #endregion

        #region Monobehaviour Callbacks
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Debug.LogWarning("There are multiple PlayerController(s) in the scene! This is not allowed!");
            }
        }

        void Start()
        {
            // get components
            
            PointerManager = pointer.GetComponent<DirectionPointer>();
            // set character to first character instance
            OnCharacterChange(null, CharacterManager.character_instances[0]);
            // subscribe to character change event
            CharacterManager.CharacterChanged += OnCharacterChange;
            // initialize states
            DefaultState = new PlayerDefaultState(this, this);
            AttackState = new PlayerAttackState(this, this);
            SkillState = new PlayerSkillState(this, this);
            MoveToZoneState = new PlayerMoveToZoneState(this, this);
            DeathState = new PlayerDeathState(this, this);
            // initialize state machine
            Initialize(DefaultState);

            // subscribe to zone change event if level manager is not null
            if (LevelManager._instance != null)
                LevelManager._instance.ZoneChanged += MoveToZoneState.OnZoneChange;
        }

        new void Update()
        {
            if (Time.timeScale <= 0f) return;
            base.Update();
        }

        new void FixedUpdate()
        {
            if (Time.timeScale <= 0f) return;
            base.FixedUpdate();
        }
        #endregion

        #region Event Listeners
        void OnCharacterChange(PlayerCharacter prevData, PlayerCharacter data)
        {
            // do not switch to character if character cannot be switched into
            if (!data.Switchable) return;
            // set data to new character
            Data = data;
            // set animator
            _anim = data.GetComponent<Animator>();
            // set behaviour to new character behaviour
            Behaviour newBehaviour = data.GetComponent<Behaviour>();
            // ensure behaviour is not null before assigning to variable
            if (newBehaviour == null) return;
            CharacterBehaviour = newBehaviour;
        }
        #endregion
    }
}
