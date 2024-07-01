using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : CoroutineState<PlayerController>
{
    Coroutine triggerSkillCoroutine;

    public PlayerSkillState(StateMachine<PlayerController> fsm, PlayerController character) : 
        base(fsm, character, character.DefaultState, character.Data.skillDuration)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // check if can trigger skill, if not, return to default state
        if (!character.CharacterBehaviour.CanTriggerSkill)
        {
            fsm.SwitchState(character.DefaultState);
            return;
        }
        // set can trigger skill to false
        character.CharacterBehaviour.CanTriggerSkill = false;
        // play skill animation
        
        // trigger skill after a certain duration
        triggerSkillCoroutine = fsm.StartCoroutine(
            WaitForSeconds(
                character.Data.skillDuration * character.Data.skillTriggerTimeFrame, 
                () => character.CharacterBehaviour?.TriggerSkill()
            )
        );
    }

    public override void Exit()
    {
        base.Exit();
        // reset can trigger skill to true, and it would automatically delay the assignment by the skill cooldown
        character.CharacterBehaviour.CanTriggerSkill = true;
        // stop trigger skill coroutine, and reset to null
        if (triggerSkillCoroutine == null) return; 
        fsm.StopCoroutine(triggerSkillCoroutine);
        triggerSkillCoroutine = null;
    }

    IEnumerator WaitForSeconds(float duration, Action callback = null)
    {
        yield return new WaitForSeconds(duration);
        callback?.Invoke();
    }
}
