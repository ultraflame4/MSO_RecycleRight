using System.Collections;
using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class BinDropState : CoroutineState<PilotrasController>
    {
        public BinDropState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.placing_duration)
        {
        }
    }
}
