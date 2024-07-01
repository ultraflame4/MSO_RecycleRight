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
        // set attack animation trigger
        character.anim?.SetTrigger("Attack");
        // trigger attack at the end of animation
        character.CharacterBehaviour?.TriggerAttack();
        // update attack duration
        duration = character.Data.attackDuration;
        // count attack duration
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        // reset attack trigger
        character.anim?.ResetTrigger("Attack");
    }
}
