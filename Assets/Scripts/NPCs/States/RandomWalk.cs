using UnityEngine;
using Patterns.FSM;
using Level;

namespace NPC.States
{
    public class RandomWalk : Movement
    {
        public float walk_distance = 2f;
        public float current_direction_strength = .55f;
        public float rand_direction_strength = .45f;


        private Vector3 current_direction = Vector3.zero;
        public RandomWalk(StateMachine<FSMRecyclableNPC> fsm, FSMRecyclableNPC character, LevelManager levelManager) : base(fsm, character, levelManager)
        {
        }



        public override void LogicUpdate()
        {

            if (navigation.reachedDestination)
            {

                // Select new rand direction;
                Vector3 randDir = new Vector3(Random.value - .5f, Random.value - .5f, 0).normalized;



                current_direction = (randDir * rand_direction_strength) + (current_direction * current_direction_strength);
                current_direction += current_edge_force;
                current_direction.Normalize();

                // Debug.Log($"Dir {current_direction}, mag {current_direction.magnitude}");
                navigation.SetDestination(transform.position + current_direction * walk_distance);
            }
            // If within buffer zone, recalculate edge force and add it to the direction (to push away from edges)
            if (levelManager.current_zone.PositionWithinBufferZone(transform.position))
            {
                CalculateEdgeForce();
                current_direction += current_edge_force;
                current_direction.Normalize();
                navigation.SetDestination(transform.position + current_direction * walk_distance);
            }

        }

    }
}
