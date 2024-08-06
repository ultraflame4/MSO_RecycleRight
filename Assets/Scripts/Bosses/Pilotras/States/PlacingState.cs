using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class PlacingState : State<PilotrasController>
    {
        public PlacingState(StateMachine<PilotrasController> fsm, PilotrasController character) : base(fsm, character)
        {
        }
    }
}
