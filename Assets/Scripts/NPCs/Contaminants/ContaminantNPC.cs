using UnityEngine;
using Level.Bins;
using NPC.Contaminants.States;
using Player;
using Interfaces;
using NPC.States;
using UnityEngine.UI;
using Patterns.FSM;
using UnityEngine.AI;

namespace NPC.Contaminant
{
    public class ContaminantNPC : FSMRecyclableNPC, ILevelEntity, IDamagable, ICleanable, IStunnable
    {
        #region States
        public Stunned state_Stunned { get; private set; }
        public DetectTarget state_Idle { get; private set; }
        public AttackRecyclable state_AttackRecyclable { get; private set; }
        public ChaseRecyclable state_ChaseRecyclable { get; private set; }
        public ChasePlayer state_ChasePlayer { get; private set; }
        public AttackPlayer state_AttackPlayer { get; private set; }
        #endregion


        #region References
        public Slider healthbar;
        public GrimeController grimeController;
        #endregion

        [Header("Config")]

        [Tooltip("Max health for this contaminant.")]
        public float maxHealth = 100f;
        [Tooltip("The speed at which the contaminant moves")]
        public float sightRange = 3f;
        [Tooltip("The attack range of the contaminant. If target is within this range, the contaminant will start attacking.")]
        public float attackRange = 1f;
        [Tooltip("The delay before each attack. in seconds")]
        public float attackDelay = .1f;
        [Tooltip("The duration of each attack. in seconds")]
        public float attackDuration = 1.5f;
        [Tooltip("The damage of each attack")]
        public float attackDamage;
        [Tooltip("Whether the contaminant can be cleaned")]
        public bool cleanable;
        [Tooltip("The prefab to instantiate when the contaminant is cleaned.")]
        public GameObject clean_prefab;

        [Tooltip("The npc data to configure this npc. Please note that this will override the above settings (on awake).")]
        public TrashNpcSO npcData;

        public override RecyclableType recyclableType => RecyclableType.OTHERS;
        public bool playerInSight => PlayerController.Instance != null && Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < sightRange;
        public bool playerInAttackRange => PlayerController.Instance != null && Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange;

        public void LoadConfig()
        {
            if (npcData == null) return;
            Debug.Log("Overriding data using npc config...");
            maxHealth = npcData.common.maxHealth;
            sightRange = npcData.common.sightRange;
            attackRange = npcData.contaminantConfig.attackRange;
            attackDelay = npcData.contaminantConfig.attackDelay;
            attackDamage = npcData.contaminantConfig.attackDamage;
            attackDuration = npcData.contaminantConfig.attackDuration;
            cleanable = npcData.contaminantConfig.cleanable;
            if (cleanable)
            {
                if (npcData.recyclableConfig.recyclablePrefab != null)
                {

                    clean_prefab = npcData.recyclableConfig.recyclablePrefab;
                    return;
                }

                Debug.LogWarning("No clean prefab set due to missing recyclable prefab in config! clean_prefab will not be overriden! This may be intended behaviour!");
            }

        }
        private void OnValidate()
        {
            LoadConfig();
        }
        private void Awake()
        {
            LoadConfig();
        }

        private void Start()
        {
            state_Idle = new DetectTarget(this);
            state_ChaseRecyclable = new ChaseRecyclable(this);
            state_AttackRecyclable = new AttackRecyclable(this);
            state_ChasePlayer = new ChasePlayer(this);
            state_AttackPlayer = new AttackPlayer(this);
            state_Stunned = new Stunned(state_Idle, this, this);
            SwitchState(state_Idle);
        }



        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, sightRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        public void OnZoneStart()
        {
            // activate the contaminant
        }

        public void Damage(float damage)
        {
            healthbar.value -= damage / maxHealth;
        }

        public void Clean(float clean_amount)
        {
            Debug.LogWarning("Contaminant cleaned! THIS IS WIP! PLEASE IMPLEMENT!");
            if (!cleanable) return;
            grimeController.GrimeAmount -= clean_amount;
            if (grimeController.GrimeAmount <= 0.1)
            {
                Instantiate(clean_prefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        public void Stun(float stun_duration)
        {
            // Debug.Log($"Stunned for {stun_duration}");
            state_Stunned.stun_timer = stun_duration;
            SwitchState(state_Stunned);
        }

        public override void SwitchState(State<FSMRecyclableNPC> nextState)
        {
            base.SwitchState(nextState);
            // Debug.Log($"Switching to {nextState}");

        }
    }
}
