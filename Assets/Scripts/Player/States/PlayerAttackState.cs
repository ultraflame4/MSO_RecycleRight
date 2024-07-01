using UnityEngine;

public class PlayerAttackState : CoroutineState<PlayerController>
{
    Coroutine triggerAttackCoroutine;

    public PlayerAttackState(StateMachine<PlayerController> fsm, PlayerController character) : 
        base(fsm, character, character.DefaultState, character.Data.attackDuration)
    {
    }

    public override void Enter()
    {
        // update attack duration
        duration = character.Data.attackDuration;
        // count attack duration
        base.Enter();
        // set attack animation trigger
        character.anim?.SetTrigger("Attack");
        // trigger attack after a certain duration
        triggerAttackCoroutine = fsm.StartCoroutine(
            WaitForSeconds(
                character.Data.attackDuration * character.Data.attackTriggerTimeFrame, 
                () => character.CharacterBehaviour?.TriggerAttack()
            )
        );
    }

    public override void Exit()
    {
        base.Exit();
        // reset attack trigger
        character.anim?.ResetTrigger("Attack");
        // stop trigger attack coroutine, and reset to null
        if (triggerAttackCoroutine == null) return; 
        fsm.StopCoroutine(triggerAttackCoroutine);
        triggerAttackCoroutine = null;
    }
}
