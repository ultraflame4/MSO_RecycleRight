using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class ToppleState : CoroutineState<PilotrasController>
    {
        float originalDamageScale = 1f;
        
        // TODO: Remove temp indicator
        SpriteRenderer sr;

        public ToppleState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.topple_duration)
        {
            sr = character.GetComponentInChildren<SpriteRenderer>();
            originalDamageScale = character.data.damageTakenScale;
        }

        public override void Enter()
        {
            base.Enter();
            if (sr != null) sr.color = Color.yellow;
            // increase damage scale to take more damage when in this state
            character.data.damageTakenScale = character.behaviourData.topple_damage_multiplier;
        }

        public override void Exit()
        {
            base.Exit();
            if (sr != null) sr.color = Color.white;
            // revert damage scale back to original
            character.data.damageTakenScale = originalDamageScale;
        }
    }
}
