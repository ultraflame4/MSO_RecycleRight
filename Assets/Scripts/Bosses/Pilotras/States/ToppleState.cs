using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class ToppleState : CoroutineState<PilotrasController>
    {
        float originalDamageScale = 1f;

        public ToppleState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.topple_duration)
        {
            originalDamageScale = character.data.damageTakenScale;
        }

        public override void Enter()
        {
            base.Enter();
            character.anim?.Play("Topple Over");
            // increase damage scale to take more damage when in this state
            character.data.damageTakenScale = character.behaviourData.topple_damage_multiplier;
        }

        public override void Exit()
        {
            base.Exit();
            character.anim?.Play("Topple Over (Reverse)");
            // revert damage scale back to original
            character.data.damageTakenScale = originalDamageScale;
        }
    }
}
