
using UnityEngine;

public class Stunned : BaseRecyclableState
{
    public Stunned(StateMachine<FSMRecyclableNPC> fsm, FSMRecyclableNPC character) : base(fsm, character)
    {
    }

    public override void Enter()
    {
        base.Enter();
        navigation.enabled=false;
    }
}