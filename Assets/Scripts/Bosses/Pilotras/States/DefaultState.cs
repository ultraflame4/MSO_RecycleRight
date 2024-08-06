using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class DefaultState : State<PilotrasController>
    {
        public DefaultState(StateMachine<PilotrasController> fsm, PilotrasController character) : base(fsm, character)
        {
        }
    }
}
