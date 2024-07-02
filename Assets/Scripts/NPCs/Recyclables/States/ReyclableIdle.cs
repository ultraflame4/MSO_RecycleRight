using System.Linq;
using UnityEngine;
using NPC.Contaminant;
using Level;

namespace NPC.Recyclable.States
{
    public class RecyclableIdle : NPC.States.RandomWalk
    {
        RecyclableNPC npc;
        public RecyclableIdle(RecyclableNPC npc) : base(npc, npc, LevelManager.Instance)
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
            bool nearbyEnemy = colliders.Any(x=>x.GetComponent<ContaminantNPC>() != null);
            if (nearbyEnemy)
            {
                npc.SwitchState(npc.state_Flee);
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        
        }
    }
}
