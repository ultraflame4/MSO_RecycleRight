using Level;
using UnityEngine;

namespace NPC.Recyclable.States
{
    public class Flee : NPC.States.Movement
    {
        RecyclableNPC npc;
        Vector3 direction;

        public Flee(RecyclableNPC npc) : base(npc, npc, LevelManager.Instance)
        {
            this.npc = npc;
        }


        public override void Enter()
        {
            base.Enter();
            navigation.ClearDestination();
        }

        public override void Exit()
        {
            base.Exit();
            navigation.ClearDestination();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            // todo in future, overlap specific circle
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, npc.sightRange);


            direction = Vector3.zero;
            foreach (var item in colliders)
            {
                var contaminant = item.GetComponent<NPC.Contaminant.ContaminantNPC>();
                if (contaminant == null) continue;

                Vector3 from_contaminant = transform.position - contaminant.transform.position;

                direction += from_contaminant.normalized * (1 / from_contaminant.sqrMagnitude); // the further away the contaminant is, the less it will affect the direction
            }
            if (direction == Vector3.zero)
            {
                npc.SwitchState(npc.state_Idle);
            }
            else
            {
                CalculateEdgeForce();
                direction.Normalize();
                direction+=current_edge_force*1.5f; // Push away from edges with a multiplier to make it more effective
                direction.Normalize();
            }
        }


        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            navigation.SetDestination(transform.position + direction * npc.sightRange);
            
        }
    }
}
