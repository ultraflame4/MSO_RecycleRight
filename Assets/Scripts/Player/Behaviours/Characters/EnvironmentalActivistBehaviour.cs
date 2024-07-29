using System.Linq;
using UnityEngine;
using Interfaces;
using NPC.Contaminant;
using Level;

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
        [SerializeField] LayerMask hitMask;

        [Header("Passive")]
        [SerializeField, Range(0f, 1f)] float cooldownDecrease = 0.2f;
        float originalSkillCooldown, lastScore, currScore;
        int scoreDifference;
        
        #region MonoBehaviour Callbacks
        void Start()
        {
            originalSkillCooldown = data.skillCooldown;
            lastScore = 0;
        }

        void Update()
        {
            if (LevelManager.Instance == null) return;
            currScore = LevelManager.Instance.GetCurrentScore();
            if (currScore > lastScore) TriggerPassive();
            lastScore = currScore;
        }
        #endregion

        #region Inherited Methods
        public override void TriggerAttack()
        {
            base.TriggerAttack();

            // get hit enemies
            Collider2D[] hits = Physics2D.OverlapCircleAll(character.transform.position, range, hitMask);
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
            // reset skill cooldown to original cooldown
            data.skillCooldown = originalSkillCooldown;

            // ensure level manager is not null
            if (LevelManager.Instance == null)
            {
                Debug.LogWarning("Level Manager could not be found! (EnvironmentalActivistBehaviour.cs)");
                return;
            }

            // get contaminants
            Collider2D[] hits = Physics2D.OverlapBoxAll(LevelManager.Instance.current_zone.transform.position, 
                LevelManager.Instance.current_zone.size, hitMask);
            ContaminantNPC[] contaminants = hits
                .Select(x => x.GetComponent<ContaminantNPC>())
                .Where(x => x != null)
                .ToArray();
            
            foreach (ContaminantNPC contaminant in contaminants)
            {
                contaminant.SpawnRecyclable();
            }
        }
        #endregion

        #region Passive
        void TriggerPassive()
        {
            // scoreDifference = (int) (currScore - lastScore);
            // if (scoreDifference <= 0) return;
            // data.skillCooldown -= cooldownDecrease * scoreDifference;
            // OverrideTriggerSkill = true;
            // CanTriggerSkill 
        }
        #endregion

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
                Quaternion.Euler(0f, 0f, rotationAngle) * (directionVector * range), Color.magenta);
            Debug.DrawLine(character.transform.position, character.transform.position + 
                Quaternion.Euler(0f, 0f, -rotationAngle) * (directionVector * range), Color.magenta);
            // draw skill range
            if (LevelManager.Instance == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(LevelManager.Instance.current_zone.transform.position, 
                LevelManager.Instance.current_zone.size);
        }
    }
}
