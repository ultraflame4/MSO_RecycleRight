using System.Linq;
using UnityEngine;
using Interfaces;
using Level;

namespace Player.Behaviours
{
    public class JanitorBehaviour : BaseRangedAttack
    {
        [SerializeField] float knockback = 5f;
        [SerializeField] float stunDuration = 0.5f;
        [SerializeField, Range(0f, 1f)] float cleanAmount = 0.25f;

        [Header("Skill")]
        [SerializeField] float skillDamage = 60f;
        [SerializeField] float skillDuration = 5f;
        [SerializeField] float skillRange = 5f;
        [SerializeField] float hitForce = 120f;
        [SerializeField] float defaultZoneHitThreshold = 1.5f;
        [SerializeField] float zoneHitThresholdScale = 1.2f;
        [SerializeField] int hitsPerAttack = 5;
        [SerializeField] int maxHitAmount = 2;
        [SerializeField] LayerMask hitMask;

        [Header("Sound Effects")]
        [SerializeField] AudioClip attackSFX;
        [SerializeField] AudioClip skillSFX;

        Collider2D[] hitColliders;
        Rigidbody2D[] hitRBs;
        LevelZone zone => LevelManager.Instance.current_zone;
        Vector2 maxPos => (Vector2) zone.center + (zone.size * 0.5f);
        Vector2 minPos => (Vector2) zone.center - (zone.size * 0.5f);
        float skillTimeElasped = 0f;

        public override void TriggerAttack()
        {
            base.TriggerAttack();
            // play sfx
            SoundManager.Instance?.PlayOneShot(attackSFX);
        }

        public override void TriggerSkill()
        {
            base.TriggerSkill();
            // play effects
            SoundManager.Instance?.PlayOneShot(skillSFX);
            LevelManager.Instance?.camera?.ShakeCamera(0.5f);
            // detect enemies within range
            hitColliders = Physics2D.OverlapCircleAll(character.pointer.position, skillRange, hitMask);
            if (hitColliders == null || hitColliders.Length <= 0) return;
            // sort by distance from self
            hitColliders = hitColliders.OrderBy(x => Vector3.Distance(x.transform.position, character.transform.position)).ToArray();
            // store rigidbody of hit objects
            hitRBs = hitColliders.Select(x => x.GetComponent<Rigidbody2D>()).ToArray();
            // reset skill time elasped
            skillTimeElasped = 0f;

            for (int i = 0; i < maxHitAmount; i++)
            {
                // ensure index is not out of range, and a rigidbody is found
                if (i >= hitColliders.Length) break;
                if (hitRBs[i] == null) continue;

                // apply damage with multiple hits
                for (int j = 0; j < hitsPerAttack; j++)
                {
                    // deal damage
                    if (hitColliders[i].TryGetComponent<IDamagable>(out IDamagable damagable)) 
                        damagable.Damage(skillDamage / hitsPerAttack);
                }
                
                // stun before adding knockback
                if (hitColliders[i].TryGetComponent<IStunnable>(out IStunnable stunnable)) stunnable.Stun(skillDuration);
                // add knockback
                hitRBs[i].AddForce((character.pointer.position - character.transform.position).normalized * hitForce, ForceMode2D.Impulse);
            }
        }

        protected override void OnProjectileHit(Projectile ctx, Collider2D other)
        {
            ctx.OnHit -= OnProjectileHit;
            // attempt to clean or damage contaminant
            CleanOrDamage(other.gameObject, cleanAmount, damage);
            // stun before adding knockback
            if (other.TryGetComponent<IStunnable>(out IStunnable stunnable))
                stunnable.Stun(stunDuration);
            // add knockback
            if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                rb.AddForce((character.pointer.position - character.transform.position).normalized * knockback, ForceMode2D.Impulse);
        }

        void HandleHit()
        {
            Vector2 position, velocity;
            float zoneHitThreshold;

            for (int i = 0; i < maxHitAmount; i++)
            {
                // ensure index is not out of range, and a rigidbody is found
                if (i >= hitColliders.Length) break;
                if (hitRBs[i] == null) continue;
                // set variables for calculation
                zoneHitThreshold = GetZoneHitThreshold(hitColliders[i]) * zoneHitThresholdScale;
                position = hitColliders[i].transform.position;
                velocity = hitRBs[i].velocity;

                // check if colliding with zone
                if (position.x >= maxPos.x - zoneHitThreshold || 
                    position.x <= minPos.x + zoneHitThreshold)
                        velocity.x *= -1f;
                if (position.y >= maxPos.y - zoneHitThreshold || 
                    position.y <= minPos.y + zoneHitThreshold)
                        velocity.y *= -1f;

                // reapply velocity
                hitRBs[i].velocity = velocity;
            }
        }

        float GetZoneHitThreshold(Collider2D collider)
        {
            if (collider.GetType() == typeof(BoxCollider2D))
            {
                BoxCollider2D boxCollider = (BoxCollider2D) collider;
                return boxCollider.size.x > boxCollider.size.y ? boxCollider.size.x : boxCollider.size.y;
            }

            if (collider.GetType() == typeof(CapsuleCollider2D))
            {
                CapsuleCollider2D capsuleCollider = (CapsuleCollider2D) collider;
                return capsuleCollider.size.x > capsuleCollider.size.y ? capsuleCollider.size.x : capsuleCollider.size.y;
            }

            if (collider.GetType() == typeof(CircleCollider2D))
            {
                CircleCollider2D circleCollider = (CircleCollider2D) collider;
                return circleCollider.radius;
            }

            return defaultZoneHitThreshold;
        }

        void FixedUpdate()
        {
            if (hitColliders == null || hitRBs == null) return;

            if (skillTimeElasped > skillDuration)
            {
                hitColliders = null;
                hitRBs = null;
                return;
            }

            skillTimeElasped += Time.fixedDeltaTime;
            HandleHit();
        }

        void OnDrawGizmosSelected()
        {
            // ensure character is not null
            if (character == null) character = GetComponentInParent<PlayerController>();
            // if character cannot be found, do not draw gizmos
            if (character == null) return;
            // show skill
            Gizmos.DrawWireSphere(character.pointer.position, skillRange);
        }
    }
}
