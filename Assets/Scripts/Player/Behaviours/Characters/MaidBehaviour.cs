using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using Interfaces;
using Level.Bins;
using NPC;
using Level;

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
        [SerializeField] TextMeshProUGUI selectedTypeText;
        [SerializeField] GameObject skillVFX;

        [Header("Skill Pulling")]
        [SerializeField] LayerMask pullableMask;
        [SerializeField] LayerMask binMask;

        [Header("Sound Effects")]
        [SerializeField] AudioClip attackSFX;
        [SerializeField] AudioClip skillStartSFX;
        [SerializeField] AudioClip skillHoldSFX;
        [SerializeField] AudioClip skillEndSFX;
        [SerializeField] float skillAudioBlendAmount = 0.15f;

        // variables to handle skill
        Collider2D[] hits;
        Coroutine tick;
        Rigidbody2D rb;
        GameObject skill_vfx_prefab;
        AudioSource skill_hold_source;
        RecyclableType? pullableType = null;
        float distance, force;
        bool skillActive = false;

        #region MonoBehaviour Callbacks
        void Start()
        {
            selectedTypeText?.gameObject.SetActive(false);
        }
        
        void LateUpdate() 
        {
            // check if skill ended early
            if (skillActive && character.currentState != character.SkillState)
            {
                EndSkill();
                return;
            }

            // check if skill has started
            if (!data.Enabled || character.currentState != character.SkillState) return;
            StartSkill();
        }

        void FixedUpdate()
        {
            PerformSkill();
        }
        #endregion

        #region Inherited Methods
        public override void TriggerAttack()
        {
            base.TriggerAttack();
            // play attack sfx
            SoundManager.Instance?.PlayOneShot(attackSFX);
        }

        public override void TriggerSkill()
        {
            base.TriggerSkill();
            // show skill vfx
            skill_vfx_prefab = Instantiate(skillVFX, character.transform.position, Quaternion.identity, transform);
            // shake camera
            LevelManager.Instance?.camera?.ShakeCamera(1f);
            // start coroutine to tick damage
            tick = StartCoroutine(CountDuration(tickSpeed, TickDamage));
            // start coroutine to count skill duration, skill duration is calculated by subtracting time from skill duration that the skill is not active
            StartCoroutine(CountDuration(data.skillDuration - (data.skillTriggerTimeFrame * data.skillDuration), EndSkill));
        }
        #endregion
        
        #region Handle Skill Start
        void StartSkill()
        {
            // do not run if skill is already active
            if (skillActive) return;
            // set skill active to true
            skillActive = true;
            // play skill sfx
            SoundManager.Instance?.PlayOneShot(skillStartSFX);
            // wait for length of skill start sfx before playing hold sfx (slightly less to blend sfx)
            StartCoroutine(CountDuration(skillStartSFX.length - skillAudioBlendAmount, () => 
                SoundManager.Instance?.Play(skillHoldSFX, out skill_hold_source, true)));
            // show selected type text
            selectedTypeText?.gameObject.SetActive(true);
            // select recyclable type
            StartCoroutine(DelaySelectRecyclableType(data.skillDuration * data.skillTriggerTimeFrame));
        }

        void SelectRecyclableType()
        {
            Collider2D[] bins = Physics2D.OverlapCircleAll(character.transform.position, skillRange, binMask);
            // find closest bin within range
            if (bins != null && bins.Length > 0)
            {
                bins = bins.OrderBy(x => Vector3.Distance(character.transform.position, x.transform.position)).ToArray();
                pullableType = bins[0].GetComponent<RecyclingBin>().recyclableType;
            }
            // set selected type text
            if (selectedTypeText == null) return;
            selectedTypeText.text = $"> {(pullableType == null ? "ALL" : pullableType)} <";
        }

        IEnumerator DelaySelectRecyclableType(float duration)
        {
            string[] recyclableTypes = Enum.GetNames(typeof(RecyclableType));
            foreach (string recyclableType in recyclableTypes)
            {
                if (selectedTypeText == null) break;
                selectedTypeText.text = $"> {recyclableType} <";
                yield return new WaitForSeconds(duration / recyclableTypes.Length);
            }
            // select recyclable type after duration
            SelectRecyclableType();
        }
        #endregion

        #region Handle Skill Hold
        void PerformSkill()
        {
            // only run if skill is active
            if (!skillActive) return;

            // get enemies that are within range
            hits = Physics2D.OverlapCircleAll(character.transform.position, skillRange, pullableMask);
            // filter out non recyclables that are not pullable
            hits = hits
                .Select(x => x.GetComponent<FSMRecyclableNPC>())
                .Where(x => x != null && (pullableType == null || x.recyclableType == (RecyclableType) pullableType))
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
        #endregion

        #region Handle Ending
        void EndSkill()
        {
            // reset skill active to false
            skillActive = false;
            // reset pullable type
            pullableType = null;
            // hide selected type text
            selectedTypeText?.gameObject.SetActive(false);
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
        #endregion

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
