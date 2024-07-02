using UnityEngine;
using Patterns.FSM;

namespace Player.BinCleaning.FSM
{
    public class MovingState : State<BinCleaning>
    {
        Vector2 move_dir;

        public MovingState(StateMachine<BinCleaning> fsm, BinCleaning character) : base(fsm, character)
        {
        }

        public override void Enter()
        {
            base.Enter();
            // play idle animation
            character.anim?.Play("Run");
            // set move boolean
            character.anim?.SetBool("IsMoving", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // check if reached target destination
            if (Vector3.Distance(character.controller.transform.position, character.transform.position) > character.binCleanRange) return;
            // when reached player, return to default state
            fsm.SwitchState(character.Default);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            // set move direction
            move_dir = (character.controller.transform.position - character.transform.position).normalized;
            // update sprite flip based on move direction
            character.currentCharacterData.renderer.flipX = move_dir.x < 0f;
            // move towards active player
            character.transform.Translate(move_dir * (character.currentCharacterData.movementSpeed / 100f) * Time.deltaTime);
        }

        public override void Exit()
        {
            base.Exit();
            // reset move boolean
            character.anim?.SetBool("IsMoving", false);
            // reset cleaning
            character.SetCleaning(false, character.controller.CharacterManager.container);
        }
    }
}
