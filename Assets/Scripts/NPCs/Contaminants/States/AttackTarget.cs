using System.Collections;
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
        public virtual IDamagable target {get;}

        public AttackTarget(ContaminantNPC npc) : base(npc, npc)
        {
            this.npc = npc;
        }


        public override void Enter()
        {
            base.Enter();
            navigation.ClearDestination();
            navigation.SetDestination(PlayerController.Instance.transform);
            // Start attack coroutine
            npc.StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            yield return new WaitForSeconds(npc.attackDelay);
            // Attack player
            target.Damage(npc.attackDamage);
            npc.SwitchState(npc.state_Idle);
        }

        public override void Exit()
        {
            base.Exit();
            navigation.ClearDestination();
        }
    }
}