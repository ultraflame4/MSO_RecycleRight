using UnityEngine;
using Patterns.FSM;

namespace Player.BinCleaning.FSM
{
    public class CleaningState : CoroutineState<BinCleaning>
    {
        public CleaningState(StateMachine<BinCleaning> fsm, BinCleaning character) : base(fsm, character, character.Moving, character.binCleanDuration)
        {
        }

        public override void Enter()
        {
            base.Enter();
            // play idle animation
            character.controller.anim?.Play("Idle");
            // reset move boolean
            character.controller.anim?.SetBool("IsMoving", false);
        }
    }
}
