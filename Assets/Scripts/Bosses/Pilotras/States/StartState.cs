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
            character.bossPeek.gameObject.SetActive(true);
            base.Enter();
            // play roaring sfx before animation ends
            character.StartCoroutine(WaitForSeconds(duration * 0.5f, character.Roar));
        }

        public override void Exit()
        {
            base.Exit();
            character.bossPeek.gameObject.SetActive(false);
        }
    }
}
