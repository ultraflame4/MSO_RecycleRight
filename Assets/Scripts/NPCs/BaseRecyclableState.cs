using UnityEngine;

public class BaseRecyclableState : State<FSMRecyclableNPC>
{
    public BaseRecyclableState(StateMachine<FSMRecyclableNPC> fsm, FSMRecyclableNPC character) : base(fsm, character)
    {
    }

    protected Navigation navigation => character.navigation;
    protected Transform transform => character.transform;
}