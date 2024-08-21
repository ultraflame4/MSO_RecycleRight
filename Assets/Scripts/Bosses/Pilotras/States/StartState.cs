using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class StartState : PhaseChangeState
    {
        public StartState(StateMachine<PilotrasController> fsm, PilotrasController character) : base(fsm, character)
        {
            playAnimation = false;
        }

        public override void Enter()
        {
            duration = character.behaviourData.spawn_duration;
            base.Enter();
            // play roaring sfx before animation ends
            character.StartCoroutine(WaitForSeconds(duration * 0.5f, character.Roar));
        }
    }
}
