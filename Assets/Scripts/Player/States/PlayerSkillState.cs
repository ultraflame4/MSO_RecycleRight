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
        // count duration from start of game to charge skill
        fsm.StartCoroutine(WaitForSeconds(character.Data.skillCooldown, () => character.canTriggerSkill = true));
    }

    public override void Enter()
    {
        base.Enter();
        // check if can trigger skill, if not, return to default state
        if (!character.canTriggerSkill)
        {
            fsm.SwitchState(character.DefaultState);
            return;
        }
        // set can trigger skill to false
        character.canTriggerSkill = false;
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
        // start coroutine to count skill cooldown
        fsm.StartCoroutine(WaitForSeconds(character.Data.skillCooldown, () => character.canTriggerSkill = true));
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
