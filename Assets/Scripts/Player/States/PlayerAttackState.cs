using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAttackState : CoroutineState<PlayerController>
{
    public PlayerAttackState(StateMachine<PlayerController> fsm, PlayerController character) : 
        base(fsm, character, character.DefaultState, character.Data.attackDuration)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // play attack animation
        // trigger attack
        character.CharacterAttack?.TriggerAttack();
    }
}
