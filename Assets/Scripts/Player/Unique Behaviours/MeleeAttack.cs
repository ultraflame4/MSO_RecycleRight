using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Behaviour
{
    // inspector fields
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float knockback = 15f;

    // perform default melee attack
    public override void TriggerAttack()
    {
        base.TriggerAttack();
        // use overlap sphere to detect enemies
        Collider[] hits = Physics.OverlapSphere(character.pointer.position, attackRange);
        // check if detected any enemies
        if (hits.Length <= 0) return;
        // add knockback to hit enemy
        hits[0].GetComponent<Rigidbody2D>()?
            .AddForce((hits[0].transform.position - transform.position).normalized * knockback, ForceMode2D.Impulse);
        // deal damage
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(character.pointer.position, attackRange);
    }
}
