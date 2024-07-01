using UnityEngine;
using Interfaces;

namespace Player.Behaviours
{
    public class BaseMeleeAttack : Behaviour
    {
        // inspector fields
        [Header("Melee Attack")]
        
        [Tooltip("Amount of damage applied to damagable enemies")]
        [SerializeField] protected float attackDamage = 5f;

        [Tooltip("Range of attack around the pointer")]
        [SerializeField] protected float attackRange = 1.5f;

        [Tooltip("Duration of stun applied on stunnable enemies")]
        [SerializeField] protected float attackStunDuration = .75f;

        [Tooltip("Amount of knockback applied to enemies that are hit")]
        [SerializeField] protected float knockback = 15f;

        [Tooltip("Set Clean Amount to 0 if attacks do not clean contaminants")]
        [SerializeField] protected float cleanAmount = 0f;

        [Tooltip("Set this to true if the attack can hit ALL enemies within range")]
        [SerializeField] protected bool areaOfEffect = false;

        [Tooltip("Layer mask of enemies that can be hit")]
        [SerializeField] protected LayerMask hitMask;

        // perform default melee attack
        public override void TriggerAttack()
        {
            base.TriggerAttack();
            // array to store all hit enemies
            Collider2D[] hits;
            // check if attacks to AoE, and detect enemies depending on that
            hits = areaOfEffect ? 
                Physics2D.OverlapCircleAll(character.pointer.position, attackRange, hitMask) : 
                new Collider2D[] { Physics2D.OverlapCircle(character.pointer.position, attackRange, hitMask) };
            
            // loop through hits list and apply hit
            foreach (Collider2D hit in hits)
            {
                // ensure hit is not null
                if (hit == null) continue;

                // if attacks can clean contaminants (clean amount > 0), try to get cleanable interface
                if (cleanAmount > 0f && hit.GetComponent<ICleanable>() != null)
                    // clean contaminant that is hit
                    hit.GetComponent<ICleanable>().Clean(cleanAmount);
                else
                    // deal damage if cannot clean
                    hit.GetComponent<IDamagable>()?.Damage(attackDamage);
                
                // stun and apply knockback to enemy that was hit
                hit.GetComponent<IStunnable>()?.Stun(attackStunDuration);
                // try add knockback by getting rigidbody and adding force in hit direction
                hit.GetComponent<Rigidbody2D>()?
                    .AddForce((character.pointer.position - transform.position).normalized * knockback, ForceMode2D.Impulse);
            }
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
