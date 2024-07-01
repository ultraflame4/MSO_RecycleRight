using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VolunteerBehaviour : BaseMeleeAttack
{
    [Header("Passive")]
    [SerializeField, Range(0f, 1f)] float passiveChance = 0.45f;

    [Header("Skill")]
    [SerializeField] float buffScale = 1.5f;
    [SerializeField] float buffDuration = 15f;

    public override void TriggerAttack()
    {
        // boolean to check if passive is triggered
        bool passiveTriggered = false;
        // check for passive
        if (Random.Range(0f, 1f) <= passiveChance)
        {
            // set passive triggered to true
            passiveTriggered = true;
            // double damage and knockback
            attackDamage *= 2;
            knockback *= 2;
            // todo: add some effect to indicate this has been triggered
        }
        // trigger base attack
        base.TriggerAttack();
        // check if passive is triggered, if so, revert changes
        if (!passiveTriggered) return;
        // revert damage and knockback
        attackDamage /= 2;
        knockback /= 2;
    }

    public override void TriggerSkill()
    {
        base.TriggerSkill();
        // set animation speed
        character.anim.speed = buffScale;
        // set animation duration
        data.attackDuration *= 1 / buffDuration;
        // subscribe to characcter change event
        character.CharacterManager.CharacterChanged += OnCharacterChange;
        // start coroutine to count buff duration
        StartCoroutine(CountBuffDuration(buffDuration));
    }

    IEnumerator CountBuffDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        // reset animation speed
        character.anim.speed = 1f;
        // reset animation duration
        data.attackDuration *= buffDuration;
        // unsubscribe to characcter change event
        character.CharacterManager.CharacterChanged -= OnCharacterChange;
    }

    // event listener to transfer changes to animator speed to new character
    void OnCharacterChange(PlayerCharacter data)
    {
        // reset animation speed
        character.anim.speed = 1f;
        // reset animation duration
        data.attackDuration *= buffDuration;
        // set animation speed
        character.anim.speed = buffScale;
        // set animation duration
        data.attackDuration *= 1 / buffDuration;
    }
}
