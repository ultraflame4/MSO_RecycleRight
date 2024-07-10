using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns.FSM;
using Level;

namespace Player.FSM
{
    public class PlayerMoveToZoneState : State<PlayerController>
    {
        LevelZone currentZone;
        Vector3 moveForce;
        Rigidbody2D rb;


        public PlayerMoveToZoneState(StateMachine<PlayerController> fsm, PlayerController character) : base(fsm, character)
        {
            // get reference to rigidbody component
            rb = character.GetComponent<Rigidbody2D>();
        }

        public override void Enter()
        {
            base.Enter();
            // play running animation
            character.anim?.Play("Run");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // check if player is within zone range
            if(!currentZone.PositionWithinZone(character.transform.position)) return;
            // start zone once player reached zone
            currentZone.StartZone();
            // return to default state once moved to zone
            fsm.SwitchState(character.DefaultState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            // move towards new zone
            Vector3 dir = moveForce - character.transform.position;
            Vector3 vel = dir.normalized * character.Data.movementSpeed * 4 * Time.deltaTime;
            rb.velocity = vel * Mathf.Clamp01( dir.sqrMagnitude);
            // check if reached target destination
            if (dir.magnitude >= .1f) return;
            rb.velocity = Vector2.zero;
        }

        // event listener (any state transition)
        public void OnZoneChange(LevelZone current_zone)
        {
            // set current zone
            currentZone = current_zone;
            // set move force
            moveForce = (Vector3) current_zone.player_startpos;
            moveForce.z = character.transform.position.z;
            // switch to this state
            fsm.SwitchState(character.MoveToZoneState);
        }
    }
}
