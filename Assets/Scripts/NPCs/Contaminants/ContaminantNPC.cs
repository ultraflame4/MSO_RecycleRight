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
    public class ContaminantNPC : FSMRecyclableNPC, IDamagable, ICleanable, IStunnable
    {
        #region States
        public Stunned state_Stunned { get; private set; }
        public Death state_Death { get; private set; }
        public DetectTarget state_Idle { get; private set; }
        public AttackRecyclable state_AttackRecyclable { get; private set; }
        public ChaseRecyclable state_ChaseRecyclable { get; private set; }
        public ChasePlayer state_ChasePlayer { get; private set; }
        public ChaseBin state_ChaseBin { get; private set; }
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
        [Tooltip("The attack range of the contaminant. If target is within this range, targe will get hit.")]
        public float attackRange = 1f;
        [Tooltip("If target is within this range, the contaminant will stop and start attacking")]
        public float startAttackRange = 1f;

        [Tooltip("The delay before each attack. in seconds")]
        public float attackDelay = .1f;
        [Tooltip("The duration of each attack. in seconds")]
        public float attackDuration = 1.5f;
        [Tooltip("The damage of each attack")]
        public float attackDamage;
        [Tooltip("Whether the contaminant can be cleaned")]
        public bool cleanable;
        [Tooltip("Whether the contaminant contains traces of food or other substances which will attract pests.")]
        public bool attract_pests = false;
        [Tooltip("The prefab to instantiate when the contaminant is cleaned.")]
        public GameObject clean_prefab;
        [Tooltip("The delay before the attack hits the target. This is used to sync the attack animation with the actual attack. In seconds.")]
        public float attack_hit_delay = 0f;

        [Tooltip("The npc data to configure this npc. Please note that this will override the above settings (on awake).")]
        public TrashNpcSO npcData;

        public override RecyclableType recyclableType => RecyclableType.OTHERS;
        public bool playerInSight => PlayerController.Instance != null && Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < sightRange;
        public bool playerInAttackRange => PlayerController.Instance != null && Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < startAttackRange;

        public override bool cause_infestation => attract_pests;

        public bool AllowCleanable => cleanable;

        private bool spawned_cleaned_prefab = false;

        public void LoadConfig()
        {
            if (npcData == null) return;

            maxHealth = npcData.common.maxHealth;
            sightRange = npcData.common.sightRange;
            attackRange = npcData.contaminantConfig.attackRange;
            attackDelay = npcData.contaminantConfig.attackDelay;
            attackDamage = npcData.contaminantConfig.attackDamage;
            attackDuration = npcData.contaminantConfig.attackDuration;
            attract_pests = npcData.contaminantConfig.attract_pests;
            attack_hit_delay = npcData.contaminantConfig.attack_hit_delay;
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
            state_Idle = new DetectTarget(this);
            state_AttackRecyclable = new AttackRecyclable(this);
            state_ChaseRecyclable = new ChaseRecyclable(this);
            state_ChaseBin = new ChaseBin(this);
            state_ChasePlayer = new ChasePlayer(this);
            state_AttackPlayer = new AttackPlayer(this);
            state_Stunned = new Stunned(state_Idle, this, this);
            state_Death = new Death(this);
        }

        private void Start()
        {
            grimeController.GrimeAmount = cleanable ? 1 : 0;
            healthbar.value = 1f;
            if (currentState != null) return;
            Initialize(state_Idle);
        }



        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, sightRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, startAttackRange);
        }

        public void Damage(float damage)
        {
            healthbar.value -= damage / maxHealth;
            if (healthbar.value < 0 && currentState != state_Death)
            {
                SwitchState(state_Death);
            }
        }

        public void Clean(float clean_amount)
        {
            if (!cleanable) return;
            // Debug.LogWarning("Contaminant cleaned! THIS IS WIP! PLEASE IMPLEMENT!");
            grimeController.GrimeAmount -= clean_amount;
            if (grimeController.GrimeAmount <= 0.1) 
                SpawnRecyclable();
        }

        public void Stun(float stun_duration)
        {
            if (healthbar.value < 0 || currentState == state_Death)
            {
                return;
            }

            if (currentState == state_Stunned) return;
            state_Stunned.stun_timer = stun_duration;
            SwitchState(state_Stunned);
        }

        public override void SpawnRecyclable()
        {
            base.SpawnRecyclable();
            if (spawned_cleaned_prefab) return;
            spawned_cleaned_prefab = true;
            var obj = Instantiate(clean_prefab, transform.position, Quaternion.identity, transform.parent);
            // carry over stun timer to child
            obj.GetComponent<IStunnable>()?.Stun(state_Stunned.stun_timer);
            Destroy(gameObject);
        }
    }
}
