using System.Linq;
using NPC;
using NPC.Contaminant;
using Patterns.FSM;
using UnityEngine;

namespace NPC.Contaminants.States
{
    public class Idle  : NPC.States.RandomWalk
    {
        ContaminantNPC npc;
        public Idle(ContaminantNPC npc) : base(npc, npc)
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
                npc.SwitchState(npc.state_Chase);
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        
        }
    }
}