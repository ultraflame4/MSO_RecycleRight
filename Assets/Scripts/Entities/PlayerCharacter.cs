using UnityEngine;

namespace Entity.Data
{
    public class PlayerCharacter : Entity
    {
        [Header("Stats")]
        [SerializeField] // LOAD BEARING NEWLINE!! DO NOT REMOVE!!!
        public float maxHealth = 100f;
        [SerializeField] public float movementSpeed = 250f;

        [Header("Attack")]
        [SerializeField, Range(0f, 1f)] public float attackTriggerTimeFrame = 0.5f;

        [Header("Skills")]
        [SerializeField] public float skillCooldown = 15f;
        [SerializeField, Range(0f, 1f)] public float skillTriggerTimeFrame = 0.5f;

        [Header("Animation Durations")]
        [SerializeField] public float attackDuration = 1.5f;
        [SerializeField] public float skillDuration = 2f;

        // hidden public properties
        private float health = 0f;
        public float Health
        {
            get { return health; }
            set { health = Mathf.Clamp(value, 0f, maxHealth); }
        }

        void Start()
        {
            // set health to max health at start of game
            Health = maxHealth;
        }
    }
}
