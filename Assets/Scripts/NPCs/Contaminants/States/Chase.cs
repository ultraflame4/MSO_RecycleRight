using NPC;
using Patterns.FSM;

namespace NPC.Contaminants.States
{
    public class Chase : NPC.BaseRecyclableState
    {
        public Chase(StateMachine<FSMRecyclableNPC> fsm, FSMRecyclableNPC character) : base(fsm, character)
        {
        }
    }
}