using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : CoroutineState<PlayerController>
{
    // todo: current state duration is hard coded, pls add data script and add character information from there
    public PlayerAttackState(StateMachine<PlayerController> fsm, PlayerController character) : 
        base(fsm, character, character.IdleState, 1.5f)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // play attack animation
    }
}
