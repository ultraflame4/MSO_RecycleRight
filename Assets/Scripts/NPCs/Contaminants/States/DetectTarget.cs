using System.Linq;
using Level;
using NPC;
using NPC.Contaminant;
using Patterns.FSM;
using UnityEngine;

namespace NPC.Contaminants.States
{
    public class DetectTarget  : NPC.States.RandomWalk
    {
        protected ContaminantNPC npc;
        public DetectTarget(ContaminantNPC npc) : base(npc, npc, LevelManager.Instance)
        {
            this.npc = npc;
        }

        public override void Enter()
        {
            base.Enter();
            
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, npc.sightRange);
            // todo in future remove any check and just check for enemy layer
            bool nearbyTarget = colliders.Any(x=>x.GetComponent<NPC.Recyclable.RecyclableNPC>() != null);
            if (nearbyTarget)
            {
                npc.SwitchState(npc.state_ChaseRecyclable);
                return;
            }

            if (npc.playerInSight)
            {
                npc.SwitchState(npc.state_ChasePlayer);
                return;
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        
        }
    }
}