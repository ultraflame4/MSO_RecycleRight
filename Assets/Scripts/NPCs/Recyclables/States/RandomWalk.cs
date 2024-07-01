using UnityEngine;
using Patterns.FSM;
using Level;

namespace NPC.Recyclable
{
    public class RandomWalk : BaseRecyclableState
    {
        public float walk_distance = 2f;
        public float current_direction_strength = .1f;
        public float rand_direction_strength = .9f;


        public LevelManager levelManager;
        private Vector3 current_direction = Vector3.zero;
        private Vector3 current_edge_force; // The force that pushes NPC away from the edge. Gets stronger the closer to the edge. (Only starts to push when within buffer zone)
        public RandomWalk(StateMachine<FSMRecyclableNPC> fsm, FSMRecyclableNPC character) : base(fsm, character)
        {
        }

        public float GetEdgeWeight(float distance)
        {
            return Mathf.Clamp01(2 * (1 - (1 / levelManager.current_zone.buffer_zone_size) * distance));
        }

        public Vector3 CalculateEdgeForce()
        {
            // The closer to the edge, the bigger the vector
            Vector3 toCentre = levelManager.current_zone.transform.position - transform.position;
            current_edge_force = new Vector3(
                                        toCentre.x * GetEdgeWeight(levelManager.current_zone.DistanceFromEdgeX(transform.position)),
                                        toCentre.y * GetEdgeWeight(levelManager.current_zone.DistanceFromEdgeY(transform.position))
                                        );
            return current_edge_force;
        }


        public override void LogicUpdate()
        {
            
            if (navigation.reachedDestination)
            {
                CalculateEdgeForce();

                // Select new rand direction;
                Vector3 randDir = new Vector3(Random.value-.5f, Random.value-.5f, 0).normalized;



                current_direction = (randDir * rand_direction_strength) + (current_direction * current_direction_strength);
                current_direction = current_direction + current_edge_force;
                current_direction.Normalize();

                // Debug.Log($"Dir {current_direction}, mag {current_direction.magnitude}");
                navigation.SetDestination(transform.position + current_direction * walk_distance);
            }

        }

        public override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, current_edge_force);
            // Gizmos.color = Color.yellow;
            // Gizmos.DrawRay(transform.position, toCentre);

        }
    }
}
