using UnityEngine;
using Level.Bins;
using NPC.Contaminants.States;
using Player;
using Interfaces;

namespace NPC.Contaminant
{
    public class ContaminantNPC : FSMRecyclableNPC, ILevelEntity, IDamagable, ICleanable
    {
        #region States
        public DetectTarget state_Idle { get; private set; }
        public AttackRecyclable state_AttackRecyclable { get; private set; }
        public ChaseRecyclable state_ChaseRecyclable { get; private set; }
        public ChasePlayer state_ChasePlayer { get; private set; }
        public AttackPlayer state_AttackPlayer { get; private set; }
        #endregion

        [Tooltip("The speed at which the contaminant moves")]
        public float move_speed;
        [Tooltip("The sight range of the contaminant.")]
        public float sightRange = 3f;
        [Tooltip("The attack range of the contaminant. If target is within this range, the contaminant will start attacking.")]
        public float attackRange = 1f;
        [Tooltip("The delay before each attack. in seconds")]
        public float attackDelay = .1f;
        [Tooltip("The damage of each attack")]
        public float attackDamage;
        [Tooltip("Whether the contaminant can be cleaned")]
        public bool cleanable;
        [Tooltip("The prefab to instantiate when the contaminant is cleaned.")]
        public GameObject clean_prefab;

        public override RecyclableType recyclableType => RecyclableType.OTHERS;
        public bool playerInSight => PlayerController.Instance != null && Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < sightRange;
        public bool playerInAttackRange => PlayerController.Instance != null && Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange;

        private void Start()
        {
            state_Idle = new DetectTarget(this);
            state_ChaseRecyclable = new ChaseRecyclable(this);
            state_AttackRecyclable = new AttackRecyclable(this);
            state_ChasePlayer = new ChasePlayer(this);
            state_AttackPlayer = new AttackPlayer(this);
            SwitchState(state_Idle);
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        public void OnZoneStart()
        {
            // activate the contaminant
        }

        public void Damage(float damage)
        {
            // todo
        }

        public void Clean(float clean_amount)
        {
            // todo
        }
    }
}
