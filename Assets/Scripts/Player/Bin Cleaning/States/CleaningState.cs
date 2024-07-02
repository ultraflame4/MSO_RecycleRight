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
            // setup cleaning
            character.SetCleaning(true, character.cleaningBin.transform);
            // play idle animation
            character.anim?.Play("Idle");
            // reset move boolean
            character.anim?.SetBool("IsMoving", false);
        }

        public override void Exit()
        {
            base.Exit();
            // completed cleaning bin
            character.cleaningBin.CompleteClean();
            // reset cleaning state of character
            character.currentCharacterData.IsCleaning = false;
        }
    }
}
