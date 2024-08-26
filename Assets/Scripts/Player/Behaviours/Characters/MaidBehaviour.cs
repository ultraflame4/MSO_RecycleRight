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
        [SerializeField] GameObject skillVFX;

        [Header("Skill Pulling")]
        [SerializeField] RecyclableType pullableType = RecyclableType.PAPER;
        [SerializeField] LayerMask pullableMask;

        [Header("Sound Effects")]
        [SerializeField] AudioClip attackSFX;
        [SerializeField] AudioClip skillStartSFX;
        [SerializeField] AudioClip skillHoldSFX;
        [SerializeField] AudioClip skillEndSFX;
        [SerializeField] float skillAudioBlendAmount = 0.15f;

        // variables to handle skill
        Collider2D[] hits;
        Coroutine tick;
        GameObject skill_vfx_prefab;
        AudioSource skill_hold_source;
        Rigidbody2D rb;
        float distance, force;
        bool skillActive = false;

        void FixedUpdate()
        {
            PerformSkill();
        }

        public override void TriggerAttack()
        {
            base.TriggerAttack();
            // play attack sfx
            SoundManager.Instance?.PlayOneShot(attackSFX);
        }

        public override void TriggerSkill()
        {
            base.TriggerSkill();
            // set skill active to true
            skillActive = true;
            // play skill sfx
            SoundManager.Instance?.PlayOneShot(skillStartSFX);
            // wait for length of skill start sfx before playing hold sfx (slightly less to blend sfx)
            StartCoroutine(CountDuration(skillStartSFX.length - skillAudioBlendAmount, () => 
                SoundManager.Instance?.Play(skillHoldSFX, out skill_hold_source, true)));
            // show skill vfx
            skill_vfx_prefab = Instantiate(skillVFX, character.transform.position, Quaternion.identity, transform);
            // start coroutine to tick damage
            tick = StartCoroutine(CountDuration(tickSpeed, TickDamage));
            // start coroutine to count skill duration, skill duration is calculated by subtracting time from skill duration that the skill is not active
            StartCoroutine(CountDuration(data.skillDuration - (data.skillTriggerTimeFrame * data.skillDuration), EndSkill));
        }

        void EndSkill()
        {
            // reset skill active to false
            skillActive = false;
            // handle resetting effects
            HandleVFX();
            HandleSFX();
            // stop damage tick
            if (tick == null) return;
            StopCoroutine(tick);
            tick = null;
        }

        void HandleVFX()
        {
            if (skill_vfx_prefab == null) return;
            // reset skill vfx prefab
            Destroy(skill_vfx_prefab);
            skill_vfx_prefab = null;
        }

        void HandleSFX()
        {
            if (skill_hold_source == null) return;
            // stop playing skill hold sfx, and play skill end sfx
            SoundManager.Instance?.PlayOneShot(skillEndSFX);
            // slightly delay stopping the sound the blend the sfx
            StartCoroutine(CountDuration(skillAudioBlendAmount, () => 
                {
                    SoundManager.Instance?.Stop(skill_hold_source);
                    skill_hold_source = null;
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
