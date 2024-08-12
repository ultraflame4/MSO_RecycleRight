using System;
using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class PhaseChangeState : CoroutineState<PilotrasController>
    {
        public event Action EndLevel;

        public PhaseChangeState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.phase_change_duration)
        {
        }

        public override void Enter()
        {
            if (character.currentPhase >= character.data.number_of_phases) 
            {
                // handle death state
                EndLevel?.Invoke();
                Debug.Log("Boss Died.");
                return;
            }

            base.Enter();
            character.Health = 0f;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            character.Health += character.data.max_health / character.behaviourData.phase_change_duration * Time.deltaTime;
        }

        public override void Exit()
        {
            base.Exit();
            character.Health = character.data.max_health;
            character.HandlePhaseChange();
        }
    }
}
