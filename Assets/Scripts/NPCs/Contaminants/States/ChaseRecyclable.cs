using Level;
using NPC;
using NPC.Contaminant;
using NPC.Recyclable;
using Patterns.FSM;
using UnityEngine;

namespace NPC.Contaminants.States
{
    public class ChaseRecyclable : NPC.States.Movement
    {
        ContaminantNPC npc;
        Vector3 direction;

        public ChaseRecyclable(ContaminantNPC npc) : base(npc, npc)
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


            // Get closest recyclable
            RecyclableNPC closestRecyclable = null;
            foreach (var item in colliders)
            {
                var target = item.GetComponent<RecyclableNPC>();
                if (target == null) continue;

                if (closestRecyclable == null)
                {
                    closestRecyclable = target;
                }

                var dist2 = (transform.position - target.transform.position).sqrMagnitude;
                var current_dist2 = (transform.position - closestRecyclable.transform.position).sqrMagnitude;

                if (dist2 < current_dist2)
                {
                    closestRecyclable = target;
                }

            }
            if (closestRecyclable == null)
            {
                npc.SwitchState(npc.state_Idle);
                return;
            }

            if (Vector3.Distance(transform.position, closestRecyclable.transform.position) < npc.startAttackRange)
            {
                npc.state_AttackRecyclable.nearestRecyclable = closestRecyclable;
                npc.SwitchState(npc.state_AttackRecyclable);
                return;
            }

            CalculateEdgeForce();
            direction = (closestRecyclable.transform.position - transform.position).normalized;
            direction += current_edge_force * 1.5f; // Push away from edges with a multiplier to make it more effective
            direction.Normalize();
        }


        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // Navigation component may be disabled!
            if (navigation != null && navigation.enabled)
            {
                navigation.SetDestination(transform.position + direction);
            }
        }
    }
}