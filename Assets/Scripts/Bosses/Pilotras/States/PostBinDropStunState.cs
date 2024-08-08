using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class PostBinDropStunState : CoroutineState<PilotrasController>
    {
        public PostBinDropStunState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.post_drop_stun_duration)
        {
        }
    }
}
