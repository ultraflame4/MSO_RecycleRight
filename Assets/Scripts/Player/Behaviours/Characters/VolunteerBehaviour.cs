using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VolunteerBehaviour : BaseMeleeAttack
{
    [Header("Passive")]
    [SerializeField, Range(0f, 1f)] float passiveChance = 0.45f;

    [Header("Skill")]
    [SerializeField] float skillDamage = 15f;
    [SerializeField] float skillRange = 5f;
    [SerializeField] float skillHealAmount = 30f;

    // skill variables
    Collider2D[] hits;
    Vector3[] hitPositions;

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
        // temp
        Debug.Log("skill triggered!");
        // detect enemies
        hits = Physics2D.OverlapCircleAll(character.transform.position, skillRange);
        // set hit position array
        hitPositions = hits
            .Select(x => x.transform.position)
            .ToArray();
        // start coroutine to stun hit enemies
        StartCoroutine(StunHitEnemies(character.Data.skillDuration - 
            (character.Data.skillDuration * character.Data.skillTriggerTimeFrame)));
    }

    IEnumerator StunHitEnemies(float duration)
    {
        float timeElapsed = 0f;
        // check time elapsed
        while (timeElapsed < duration)
        {
            // lock all hit enemy positions in hit list
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].transform.position = hitPositions[i];
            }
            // increment time elapsed
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        // reset hits and hit positions list
        hits = null;
        hitPositions = null;
        Debug.Log("ended");
    }

    new void OnDrawGizmosSelected() 
    {
        // draw gizmos of base class
        base.OnDrawGizmosSelected();
        // ensure character is not null
        if (character == null) return;
        // show skill range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(character.transform.position, skillRange);
    }
}
