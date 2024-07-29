using System.Collections;
using System.Linq;
using Interfaces;
using Level;
using NPC;
using NPC.Contaminant;
using NPC.Recyclable;
using Patterns.FSM;
using Player;
using UnityEngine;

namespace NPC.Contaminants.States
{
    public class AttackTarget : BaseRecyclableState
    {

        protected ContaminantNPC npc;
        public virtual IDamagable target { get; }

        private Coroutine attackCoroutine;
        public static int animParamTriggerAttack = Animator.StringToHash("Attack");


        public AttackTarget(ContaminantNPC npc) : base(npc, npc)
        {
            this.npc = npc;
        }


        public override void Enter()
        {
            base.Enter();
            // Navigation component may be disabled!
            if (navigation != null && navigation.enabled)
            {
                navigation.ClearDestination();
                navigation.StopVelocity();
            }

            // Start attack coroutine
            if (attackCoroutine != null) // if coroutine active, stop it
            {
                npc.StopCoroutine(attackCoroutine);
            }
            attackCoroutine = npc.StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            yield return new WaitForSeconds(npc.attackDelay);
            // Attack target
            // Debug.Log($"Attacking target {target}");
            npc.animator?.SetTrigger(animParamTriggerAttack);
        }

        protected virtual void OnAttackTarget()
        {
            target?.Damage(npc.attackDamage);
        }

        public void TriggerHit()
        {
            if (target == null) return;
            Debug.Log($"Hitting target {target}");
            if (fsm.currentState != this) return;

            OnAttackTarget();
        }

        public void EndAttack()
        {
            npc.SwitchState(npc.state_Idle);
        }

        public override void Exit()
        {
            base.Exit();
            navigation.ClearDestination();
            if (attackCoroutine != null) // On exit state, stop all coroutines to prevent unexpected behaviours. (The state may end before coroutine does)
            {
                npc.StopCoroutine(attackCoroutine);
            }
            npc.animator?.ResetTrigger(animParamTriggerAttack);
        }
    }
}