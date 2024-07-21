using UnityEngine;
using Interfaces;
using Player;

namespace Entity.Data
{
    public class PlayerCharacter : Entity, IDamagable
    {
        // inspector fields
        [SerializeField] PlayerCharacterSO objectData;

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
        [HideInInspector] public float deathDuration = 3.5f;
        #endregion

        #region Booleans
        [HideInInspector] public bool IsCleaning = false;
        [HideInInspector] public bool OverrideSwitchable = false;
        #endregion

        #region Multipliers
        [HideInInspector] public float movementMultiplier = 1f;
        [HideInInspector] public float attackMultiplier = 1f;
        [HideInInspector] public float skillCooldownMultiplier = 1f;
        #endregion

        #region Multiplied Data
        public float netMovementSpeed => movementSpeed * movementMultiplier;
        public float netAttackDuration => attackDuration * attackMultiplier;
        public float netSkillCooldown => skillCooldown * skillCooldownMultiplier;
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

        #region Interface Methods
        public void Damage(float damage)
        {
            // apply damage
            Health -= damage;
            // check if died, if so, switch to death state
            if (PlayerController.Instance == null || Health > 0) return;
            PlayerController.Instance.SwitchState(PlayerController.Instance.DeathState);
        }
        #endregion

        #region Other Methods
        public void SetData()
        {
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
            deathDuration = objectData.deathDuration;
            Health = maxHealth;
        }
        #endregion
        
        #region MonoBehaviour Callback
        protected override void Awake()
        {
            SetData();
            base.Awake();
        }
        #endregion
    }
}
