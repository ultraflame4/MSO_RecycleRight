using UnityEngine;
using Patterns.FSM;

namespace Player.BinCleaning.FSM
{
    public class DefaultState : State<BinCleaning>
    {
        public DefaultState(StateMachine<BinCleaning> fsm, BinCleaning character) : base(fsm, character)
        {
        }
    }
}
