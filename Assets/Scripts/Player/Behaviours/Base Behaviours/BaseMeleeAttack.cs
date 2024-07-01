using UnityEngine;
using Interfaces;

namespace Player.Behaviours
{
    public class BaseMeleeAttack : Behaviour
    {
        // inspector fields
        [Header("Melee Attack")]
        [SerializeField] protected float attackDamage = 5f;
        [SerializeField] protected float attackRange = 1.5f;
        [SerializeField] protected float attackStunDuration = .75f;
        [SerializeField] protected float knockback = 15f;
        [SerializeField] protected LayerMask hitMask;

        // perform default melee attack
        public override void TriggerAttack()
        {
            base.TriggerAttack();
            // todo: add layer to enemy, and only detect enemy layer
            // use overlap sphere to detect enemies
            Collider2D hit = Physics2D.OverlapCircle(character.pointer.position, attackRange, hitMask);
            // check if detected any enemies
            if (hit == null) return;

            // deal damage and stun enemies that are hit
            hit.GetComponent<IDamagable>()?.Damage(attackDamage);
            hit.GetComponent<IStunnable>()?.Stun(attackStunDuration);
            // add knockback to hit enemy
            hit.GetComponent<Rigidbody2D>()?
                .AddForce((character.pointer.position - transform.position).normalized * knockback, ForceMode2D.Impulse);

        }

        protected void OnDrawGizmosSelected()
        {
            // ensure character is not null
            if (character == null) character = GetComponentInParent<PlayerController>();
            // if character cannot be found, do not draw gizmos
            if (character == null) return;
            // show attack range
            Gizmos.DrawWireSphere(character.pointer.position, attackRange);
        }
    }
}
