using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class PhaseChangeState : CoroutineState<PilotrasController>
    {
        public PhaseChangeState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.phase_change_duration)
        {
        }

        public override void Enter()
        {
            if (character.currentPhase >= character.data.number_of_phases) 
            {
                fsm.SwitchState(character.DeathState);
                return;
            }

            base.Enter();
            character.Health = 0f;
            // do not allow taking damage in this state
            character.data.damageTakenScale = 0f;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            character.Health += character.data.max_health / duration * Time.deltaTime;
        }

        public override void Exit()
        {
            base.Exit();
            character.Health = character.data.max_health;
            character.HandlePhaseChange();
            // allow taking damage again when exitting
            character.data.damageTakenScale = 1f;
        }
    }
}
