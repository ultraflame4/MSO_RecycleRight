using UnityEngine;
using Patterns.FSM;
using Level;

namespace NPC.States
{
    public class Movement : BaseRecyclableState
    {
        public LevelManager levelManager => LevelManager.Instance;
        public static int animParamBoolWalk = Animator.StringToHash("Walk");
        protected Vector3 current_edge_force; // The force that pushes NPC away from the edge. Gets stronger the closer to the edge. (Only starts to push when within buffer zone)
        public Movement(StateMachine<FSMRecyclableNPC> fsm, FSMRecyclableNPC character) : base(fsm, character)
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

        public override void Enter()
        {
            base.Enter();
            character.animator?.SetBool(animParamBoolWalk, true);
        }
        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, current_edge_force);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // Navigation component may be disabled!
            if (navigation == null) return;

            if (character.spriteRenderer) character.spriteRenderer.flipX = navigation.flipX;
        }
        public override void Exit()
        {
            base.Exit();
            character.animator?.SetBool(animParamBoolWalk, false);
        }
    }
}
