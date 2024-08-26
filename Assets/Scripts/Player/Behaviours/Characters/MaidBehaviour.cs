using System.Linq;
using UnityEngine;
using Level.Bins;
using NPC;
using Interfaces;

namespace Player.Behaviours
{
    public class MaidBehaviour : BaseMeleeAttack
    {
        [Header("Skill Tick")]
        [SerializeField] float tickDamage = 1f;
        [SerializeField] float tickStunDuration = 0.15f;
        [SerializeField] float tickSpeed = 1f;

        [Header("Skill Fields")]
        [SerializeField] float skillRange = 5f;
        [SerializeField] float minDropoffRange = 3f;
        [SerializeField] float skillPullForce = 25f;

        [Header("Skill Pulling")]
        [SerializeField] RecyclableType pullableType = RecyclableType.PAPER;
        [SerializeField] LayerMask pullableMask;

        // variables to handle skill
        Collider2D[] hits;
        Coroutine tick;
        Rigidbody2D rb;
        float distance, force;
        bool skillActive = false;

        void FixedUpdate()
        {
            PerformSkill();
        }

        public override void TriggerSkill()
        {
            base.TriggerSkill();
            // set skill active to true
            skillActive = true;
            // start coroutine to tick damage
            tick = StartCoroutine(CountDuration(tickSpeed, TickDamage));
            // start coroutine to count skill duration, skill duration is calculated by subtracting time from skill duration that the skill is not active
            StartCoroutine(CountDuration(data.skillDuration - (data.skillTriggerTimeFrame * data.skillDuration), () => 
                {
                    // reset skill active to false
                    skillActive = false;
                    // stop damage tick
                    if (tick == null) return;
                    StopCoroutine(tick);
                    tick = null;
                }
            ));
        }

        void PerformSkill()
        {
            // only run if skill is active
            if (!skillActive) return;

            // get enemies that are within range
            hits = Physics2D.OverlapCircleAll(character.transform.position, skillRange, pullableMask);
            // filter out non recyclables that are not pullable
            hits = hits
                .Select(x => x.GetComponent<FSMRecyclableNPC>())
                .Where(x => x != null && x.recyclableType == pullableType)
                .Select(x => x.GetComponent<Collider2D>())
                .Where(x => x != null)
                .ToArray();
            
            // pull all hit enemy towards self
            foreach (Collider2D hit in hits)
            {
                // get rigidbody of component to apply force
                rb = hit.GetComponent<Rigidbody2D>();
                // ensure enemy that was hit has a rigidbody component
                if (rb == null) continue;
                // get distance of hit collider
                distance = Vector3.Distance(character.transform.position, hit.transform.position);
                // calculate force to add
                force = distance <= minDropoffRange ? 1f : 1f - Mathf.Clamp01(distance - minDropoffRange / skillRange - minDropoffRange);
                // pull enemy towards self
                rb.AddForce((character.transform.position - hit.transform.position).normalized * 
                    force * skillPullForce);
            }
        }

        void TickDamage()
        {
            // deal damage and stun all hit enemies within range
            foreach (Collider2D hit in hits)
            {
                hit.GetComponent<IDamagable>()?.Damage(tickDamage);
                hit.GetComponent<IStunnable>()?.Stun(tickStunDuration);
            }
            // start another coroutine to tick again
            tick = StartCoroutine(CountDuration(tickSpeed, TickDamage));
        }

        new void OnDrawGizmosSelected() 
        {
            // draw gizmos of base state
            base.OnDrawGizmosSelected();
            // ensure character is not null
            if (character == null) character = GetComponentInParent<PlayerController>();
            // if character cannot be found, do not draw gizmos
            if (character == null) return;
            // draw skill range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(character.transform.position, skillRange);
        }
    }
}
