using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolunteerBehaviour : BaseMeleeAttack
{
    [Header("Passive")]
    [SerializeField, Range(0f, 1f)] float passiveChance = 0.45f;

    [Header("Skill")]
    [SerializeField] float skillDamage = 15f;
    [SerializeField] float skillRange = 5f;
    [SerializeField] float skillHealAmount = 30f;

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
}
