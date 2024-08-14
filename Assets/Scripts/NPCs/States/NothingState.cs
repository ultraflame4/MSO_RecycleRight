using UnityEngine;
using Patterns.FSM;
using Level;

namespace NPC.States
{
    /// <summary>
    ///  This state does nothing :)
    /// </summary>
    public class NothingState : BaseRecyclableState
    {

        public NothingState(StateMachine<FSMRecyclableNPC> fsm, FSMRecyclableNPC character) : base(fsm, character)
        {
        }



        public override void LogicUpdate()
        {

        }

    }
}
