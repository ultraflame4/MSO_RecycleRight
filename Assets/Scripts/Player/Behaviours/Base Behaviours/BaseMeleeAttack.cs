using System.Linq;
using UnityEngine;
using Interfaces;
using NPC.Contaminant;

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
        [SerializeField, Range(0f, 1f)] protected float cleanAmount = 0f;

        [Tooltip("Set this to true if the attack can hit ALL enemies within range")]
        [SerializeField] protected bool areaOfEffect = false;

        [Tooltip("Layer mask of enemies that can be hit")]
        [SerializeField] protected LayerMask hitMask;

        [Header("VFX")]
        [Tooltip("Effect to spawn when attack is triggered")]
        [SerializeField] protected GameObject hitEffects;

        // perform default melee attack
        public override void TriggerAttack()
        {
            base.TriggerAttack();
            // array to store all hit enemies
            Collider2D[] hits;
            // detect enemies in both direction of pointer, and area around self
            hits = Physics2D.OverlapCircleAll(character.pointer.position, attackRange, hitMask)
                .Concat(Physics2D.OverlapCircleAll(character.transform.position, attackRange, hitMask))
                .ToArray();
            // sort by distance from self
            hits = hits.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToArray();
            // only keep first element if AoE is false
            if (!areaOfEffect && hits.Length > 0) hits = new Collider2D[] { hits[0] };

            // loop through hits list and apply hit
            foreach (Collider2D hit in hits)
            {
                // ensure hit is not null
                if (hit == null) continue;

                // attempt to get reference to contaminant fsm
                ContaminantNPC contaminant = hit.GetComponent<ContaminantNPC>();
                // clean contaminant that is hit if it is cleanable
                CleanOrDamage(hit.gameObject, cleanAmount, attackDamage);
                
                // stun and apply knockback to enemy that was hit
                hit.GetComponent<IStunnable>()?.Stun(attackStunDuration);
                // try add knockback by getting rigidbody and adding force in hit direction
                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                if (rb == null) return;
                rb.AddForce((character.pointer.position - character.transform.position).normalized * knockback, ForceMode2D.Impulse);
            }

            // spawn hit vfx
            // ensure hit effects prefab is provided
            if (hitEffects == null) return;
            // spawn hit vfx
            GameObject vfx = Instantiate(
                hitEffects, 
                character.pointer.position, 
                Quaternion.identity, 
                character.transform
            );
            // set vfx direction
            vfx.transform.up = character.pointer.up;
        }

        protected void OnDrawGizmosSelected()
        {
            // ensure character is not null
            if (character == null) character = GetComponentInParent<PlayerController>();
            // if character cannot be found, do not draw gizmos
            if (character == null) return;
            // show attack range
            Gizmos.DrawWireSphere(character.pointer.position, attackRange);
            Gizmos.DrawWireSphere(character.transform.position, attackRange);
        }
    }
}
