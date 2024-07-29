using System.Linq;
using UnityEngine;
using Interfaces;
using NPC.Contaminant;

namespace Player.Behaviours
{
public class EnvironmentalActivistBehaviour : Behaviour
    {
        [Header("Attack")]
        [SerializeField] float attackDamage = 5f;
        [SerializeField] float attackStunDuration = .5f;
        [SerializeField] float range = 2.5f;
        [SerializeField, Range(0f, 1f)] float angle = 0.45f;
        [SerializeField] float knockback = 5f;
        [SerializeField, Range(0f, 1f)] float cleanAmount = 0.6f;

        public override void TriggerAttack()
        {
            base.TriggerAttack();

            // get hit enemies
            Collider2D[] hits = Physics2D.OverlapCircleAll(character.transform.position, range);
            // filter based on angle
            hits = hits
                .Where(x => 
                    {
                        float dot = Vector3.Dot((character.pointer.position - character.transform.position).normalized, 
                            (x.transform.position - character.transform.position).normalized);
                        return dot <= 1f && dot >= angle;
                    })
                .ToArray();

            // loop through hits list and apply hit
            foreach (Collider2D hit in hits)
            {
                // ensure hit is not null
                if (hit == null) continue;

                // attempt to get reference to contaminant fsm
                ContaminantNPC contaminant = hit.GetComponent<ContaminantNPC>();
                // clean contaminant that is hit if it is cleanable
                if (cleanAmount > 0f && contaminant != null && contaminant.cleanable)
                    hit.GetComponent<ICleanable>()?.Clean(cleanAmount);
                else
                    // deal damage if cannot clean
                    hit.GetComponent<IDamagable>()?.Damage(attackDamage);
                
                // stun and apply knockback to enemy that was hit
                hit.GetComponent<IStunnable>()?.Stun(attackStunDuration);
                // try add knockback by getting rigidbody and adding force in hit direction
                hit.GetComponent<Rigidbody2D>()?
                    .AddForce((character.pointer.position - character.transform.position).normalized * knockback, ForceMode2D.Impulse);
            }
        }

        public override void TriggerSkill()
        {
            base.TriggerSkill();
        }

        protected void OnDrawGizmosSelected() 
        {
             // ensure character is not null
            if (character == null) character = GetComponentInParent<PlayerController>();
            // if character cannot be found, do not draw gizmos
            if (character == null) return;
            // draw attack range
            Gizmos.DrawWireSphere(character.transform.position, range);
            // draw angle boundaries
            float rotationAngle = 90f * (1f - angle);
            Vector3 directionVector = (character.pointer.position - character.transform.position).normalized;
            Debug.DrawLine(character.transform.position, character.transform.position + 
                Quaternion.Euler(0f, 0f, rotationAngle) * (directionVector * range), Color.yellow);
            Debug.DrawLine(character.transform.position, character.transform.position + 
                Quaternion.Euler(0f, 0f, -rotationAngle) * (directionVector * range), Color.yellow);
        }
    }
}
