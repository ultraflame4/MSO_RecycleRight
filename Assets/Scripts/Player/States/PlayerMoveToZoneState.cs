using UnityEngine;
using Patterns.FSM;
using Level;

namespace Player.FSM
{
    public class PlayerMoveToZoneState : State<PlayerController>
    {
        Vector3 dest;
        Rigidbody2D rb;

        public PlayerMoveToZoneState(StateMachine<PlayerController> fsm, PlayerController character) : base(fsm, character)
        {
            // get reference to rigidbody component
            rb = character.GetComponent<Rigidbody2D>();
        }

        public override void Enter()
        {
            base.Enter();
            // disable collider when changing zones
            character.Data.collider.enabled = false;
            character.obstacleCollider.enabled = false;
        }

        public override void Exit()
        {
            base.Exit();
            // reset running animation
            character.anim?.SetBool("IsMoving", false);
            // enable collider after zone start
            character.Data.collider.enabled = true;
            character.obstacleCollider.enabled = true;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            // move towards new zone
            Vector3 dir = dest - character.transform.position;
            Vector3 vel = dir.normalized * character.Data.movementSpeed * 4 * Time.deltaTime;
            rb.velocity = vel * Mathf.Clamp01(dir.sqrMagnitude);
            
            // play running animation
            character.anim?.SetBool("IsMoving", true);
            // update sprite flip
            character.Data.renderer.flipX = vel.x < 0f;

            // check if reached target destination
            if (dir.magnitude >= .1f) return;
            rb.velocity = Vector2.zero;
            
            // return to default state once moved to zone
            fsm.SwitchState(character.DefaultState);
        }

        // event listener (any state transition)
        public void OnZoneChange(LevelZone current_zone)
        {
            // ignore zone 1
            if (LevelManager.Instance.current_zone_index == 0) return;
            // set move force
            dest = (Vector3)current_zone.player_startpos;
            dest.z = character.transform.position.z;
            // switch to this state
            fsm.SwitchState(character.MoveToZoneState);
        }
    }
}
