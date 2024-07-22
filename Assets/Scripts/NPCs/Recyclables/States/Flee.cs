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
            // Navigation component may be disabled!
            if (navigation != null && navigation.enabled)
            {
                navigation.ClearDestination();
            }
        }

        public override void Exit()
        {
            base.Exit();
            // Navigation component may be disabled!
            if (navigation != null && navigation.enabled)
            {
                navigation.ClearDestination();
            }
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

            // If no contaminant is found, switch to idle
            if (direction == Vector3.zero)
            {
                npc.SwitchState(npc.state_Idle);
            }
            else // Otherwise runaway (normalise direction)
            {

                // Normalise direction force
                direction.Normalize();

                // If within buffer zone, recalculate edge force and add it to the direction (to push away from edges)
                if (levelManager.current_zone.PositionWithinBufferZone(transform.position))
                {
                    CalculateEdgeForce();
                    direction += current_edge_force;
                    direction.Normalize();
                }
            }
        }


        public override void LogicUpdate()
        {
            base.LogicUpdate();


            // Navigation component may be disabled!
            if (navigation != null && navigation.enabled)
            {
                // Update destination
                navigation.SetDestination(transform.position + direction * npc.sightRange);
            }
        }
    }
}
