using System.Linq;
using Level;
using Level.Bins;
using NPC;
using NPC.Contaminant;
using Patterns.FSM;
using UnityEngine;

namespace NPC.Contaminants.States
{
    public class DetectTarget  : NPC.States.RandomWalk
    {
        protected ContaminantNPC npc;
        public DetectTarget(ContaminantNPC npc) : base(npc, npc)
        {
            this.npc = npc;
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
            bool nearbyBin = colliders.Any(x=>x.GetComponent<RecyclingBin>());
            if (nearbyBin){
                npc.SwitchState(npc.state_ChaseBin);
                return;
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        
        }
    }
}