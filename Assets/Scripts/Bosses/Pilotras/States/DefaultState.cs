using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class DefaultState : State<PilotrasController>
    {
        public DefaultState(StateMachine<PilotrasController> fsm, PilotrasController character) : base(fsm, character)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (character.currentPhase > 1 && character.LaneAttackState.CanEnter)
            {
                fsm.SwitchState(character.LaneAttackState);
                return;
            }

            if (character.PlacingState.CanEnter)
            {
                fsm.SwitchState(character.PlacingState);
                return;
            }

            if (character.BinDropState.CanEnter)
            {
                fsm.SwitchState(character.BinDropState);
                return;
            }
        }
    }
}
