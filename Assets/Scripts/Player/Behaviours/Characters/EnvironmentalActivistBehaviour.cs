using System.Linq;
using UnityEngine;
using Interfaces;
using NPC.Contaminant;
using Level;
using UnityEngine.Serialization;

namespace Player.Behaviours
{
    public class EnvironmentalActivistBehaviour : Behaviour
    {
        [Header("Attack")]
        [SerializeField] float attackDamage = 5f;
        [SerializeField, Range(0f, 1f)] float damageDropoffRange = 0.25f;
        [SerializeField, Range(0f, 1f)] float minDropoffScale = 0.3f;
        [SerializeField] float attackStunDuration = .5f;
        [SerializeField] float range = 2.5f;
        [SerializeField, Range(0f, 1f)] float angle = 0.45f;
        [SerializeField] float knockback = 5f;
        [SerializeField, Range(0f, 1f)] float cleanAmount = 0.6f;
        [SerializeField] LayerMask hitMask;
        [SerializeField] GameObject attackRangeIndicator;

        [Header("Passive")]
        [SerializeField] float cooldownDecrease = 2f;

        [Header("Effects")]
        [SerializeField] AudioClip attackSFX;
        [SerializeField] AudioClip attackCloseSFX;
        [SerializeField] AudioClip skillSFX;
        [Tooltip("VFX to spawn when skill is used"), FormerlySerializedAs("skillVFXForContaminant")]
        [SerializeField] GameObject skillVFX;

        GameObject indicatorPrefab;
        SpriteRenderer pointerSprite;
        bool replaceIndicator = true;
        bool attackClose = false;
        float lastScore, currScore, dropoffScale, distance;
        int scoreDifference;

        #region MonoBehaviour Callbacks
        protected override void Awake()
        {
            // call awake in base state
            base.Awake();
            // reset variables
            lastScore = 0;
            // instantiate attack range indicator prefab
            if (attackRangeIndicator == null) return;
            indicatorPrefab = Instantiate(
                attackRangeIndicator,
                character.transform.position,
                Quaternion.identity,
                character.pointer
            );
            // get reference to pointer sprite
            pointerSprite = character.pointer.GetComponentInChildren<SpriteRenderer>();
        }

        void Update()
        {
            // check whether to show attack indicator (when character is active)
            indicatorPrefab?.SetActive(data.Enabled && replaceIndicator);
            if (indicatorPrefab != null) pointerSprite.enabled = !data.Enabled || !replaceIndicator;
            // check if passive is triggered
            currScore = LevelManager.Instance.GetCurrentScore();
            if (currScore > lastScore) TriggerPassive();
            lastScore = currScore;
        }
        #endregion

        #region Inherited Methods
        public override void TriggerAttack()
        {
            base.TriggerAttack();

            // show original indicator when triggering attack
            replaceIndicator = false;
            StartCoroutine(CountDuration(data.attackDuration - (data.attackDuration * data.attackTriggerTimeFrame),
                () => replaceIndicator = true));

            // get hit enemies
            Collider2D[] hits = Physics2D.OverlapCircleAll(character.transform.position, range, hitMask);
            if (hits.Length <= 0) return;
            // reset attack close boolean
            attackClose = false;

            // filter based on angle
            hits = hits
                .Where(x =>
                    {
                        float dot = Vector3.Dot((character.pointer.position - character.transform.position).normalized,
                            (x.transform.position - character.transform.position).normalized);
                        return (dot <= 1f && dot >= angle) ||
                            Vector3.Distance(x.transform.position, character.transform.position) <= (range * damageDropoffRange);
                    })
                .ToArray();

            // loop through hits list and apply hit
            foreach (Collider2D hit in hits)
            {
                // ensure hit is not null
                if (hit == null) continue;

                // calculate dropoff
                distance = Vector3.Distance(hit.transform.position, character.transform.position);
                dropoffScale = distance <= (range * damageDropoffRange) ? 2f : Mathf.Clamp(minDropoffScale, 1f, 1f - (distance / range));
                // check if there is a close ranged attack
                if (!attackClose) attackClose = dropoffScale > 1f;

                // attempt to get reference to contaminant fsm
                ContaminantNPC contaminant = hit.GetComponent<ContaminantNPC>();

                CleanOrDamage(hit.gameObject, cleanAmount, attackDamage * dropoffScale);

                // stun and apply knockback to enemy that was hit
                hit.GetComponent<IStunnable>()?.Stun(attackStunDuration * dropoffScale);
                // try add knockback by getting rigidbody and adding force in hit direction
                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                if (rb == null) continue;
                rb.AddForce((character.pointer.position - character.transform.position).normalized *
                    knockback * dropoffScale, ForceMode2D.Impulse);
            }

            // play sfx depending on if an enemy is hit in close range
            SoundManager.Instance?.PlayOneShot((attackClose ? attackCloseSFX : attackSFX));
            // shake camera if hit a close ranged attack
            if (attackClose) LevelManager.Instance?.camera?.ShakeCamera(0.15f);
        }

        public override void TriggerSkill()
        {
            base.TriggerSkill();

            // show original indicator when triggering attack
            replaceIndicator = false;
            StartCoroutine(CountDuration(data.skillDuration - (data.skillDuration * data.skillTriggerTimeFrame),
                () => replaceIndicator = true));

            // ensure level manager is not null
            if (LevelManager.Instance == null)
            {
                Debug.LogWarning("Level Manager could not be found! (EnvironmentalActivistBehaviour.cs)");
                return;
            }

            // play sfx
            SoundManager.Instance?.PlayOneShot(skillSFX);
            // get contaminants
            Collider2D[] hits = Physics2D.OverlapBoxAll(LevelManager.Instance.current_zone.center,
                LevelManager.Instance.current_zone.size, 0f, hitMask);
            ContaminantNPC[] contaminants = hits
                .Select(x => x.GetComponent<ContaminantNPC>())
                .Where(x => x != null)
                .ToArray();


            Instantiate(skillVFX, transform);

            foreach (ContaminantNPC contaminant in contaminants)
            {
                var recyclableVariant = contaminant.npcData.recyclableConfig.recyclablePrefab;
                if (recyclableVariant == null) continue;
                // spawn vfx
                Instantiate(skillVFX, contaminant.transform.position, Quaternion.identity);
                // Manually instantiate clean variant!
                var obj = Instantiate(recyclableVariant, contaminant.transform.position, Quaternion.identity, contaminant.transform.parent);
                // carry over stun timer to child
                obj.GetComponent<IStunnable>()?.Stun(contaminant.state_Stunned.stun_timer);
                Destroy(contaminant.gameObject);
            }
        }
        #endregion

        #region Passive
        void TriggerPassive()
        {
            scoreDifference = (int)(currScore - lastScore);
            if (scoreDifference <= 0) return;
            CooldownElasped += cooldownDecrease * scoreDifference;
            if (CooldownElasped <= data.skillCooldown) return;
            CooldownElasped = data.skillCooldown;
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
            // draw dropoff range
            Gizmos.DrawWireSphere(character.transform.position, range * damageDropoffRange);
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
