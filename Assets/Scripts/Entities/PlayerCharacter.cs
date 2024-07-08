using UnityEngine;

namespace Entity.Data
{
    public class PlayerCharacter : Entity
    {
        // inspector fields
        [SerializeField] PlayerCharacterObject objectData;

        #region Object Data
        // stats
        [HideInInspector] public float maxHealth = 100f;
        [HideInInspector] public float movementSpeed = 250f;

        // attack
        [HideInInspector] public float attackTriggerTimeFrame = 0.5f;

        // skills
        [HideInInspector] public float skillCooldown = 15f;
        [HideInInspector] public float skillTriggerTimeFrame = 0.5f;
        [HideInInspector] public Sprite skillIcon;

        // animation durations
        [HideInInspector] public float attackDuration = 1.5f;
        [HideInInspector] public float skillDuration = 2f;
        #endregion

        #region Other Variables
        // multipliers
        [HideInInspector] public float movementMultiplier = 1f;
        [HideInInspector] public float attackMultiplier = 1f;

        // booleans
        [HideInInspector] public bool IsCleaning = false;
        [HideInInspector] public bool OverrideSwitchable = false;
        #endregion

        #region Properties
        private float health = 0f;
        public float Health
        {
            get { return health; }
            set { health = Mathf.Clamp(value, 0f, maxHealth); }
        }

        public bool Switchable => !OverrideSwitchable && !(IsCleaning || Health <= 0);
        #endregion
        
        #region MonoBehaviour Callback
        new void Awake()
        {
            // set data
            // base entity data
            characterName = objectData.characterName;
            characterDesc = objectData.characterDesc;
            characterSprite = objectData.characterSprite;
            // character data
            maxHealth = objectData.maxHealth;
            movementSpeed = objectData.movementSpeed;
            attackTriggerTimeFrame = objectData.attackTriggerTimeFrame;
            skillCooldown = objectData.skillCooldown;
            skillTriggerTimeFrame = objectData.skillTriggerTimeFrame;
            skillIcon = objectData.skillIcon;
            attackDuration = objectData.attackDuration;
            skillDuration = objectData.skillDuration;
            // run base method
            base.Awake();
        }
        
        void Start()
        {
            // set health to max health at start of game
            Health = maxHealth;
        }
        #endregion
    }
}
