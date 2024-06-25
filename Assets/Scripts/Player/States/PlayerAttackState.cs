using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : CoroutineState<PlayerController>
{
    public PlayerAttackState(StateMachine<PlayerController> fsm, PlayerController character) : 
        base(fsm, character, character.IdleState, character.Data.attackDuration)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // play attack animation
    }
}
