using UnityEngine;
using Interfaces;

namespace Player.Behaviours
{
    public class JanitorBehaviour : BaseRangedAttack
    {
        [Header("Janitor Attack")]
        [SerializeField] float knockback = 5f;
        [SerializeField] float stunDuration = 0.5f;
        [SerializeField, Range(0f, 1f)] float cleanAmount = 0.25f;

        // Start is called before the first frame update
        void Start()
        {
            OnLaunch += SetClean;
        }

        void SetClean(Projectile projectile)
        {
            projectile.OnHit += OnProjectileHit;
        }

        void OnProjectileHit(Projectile ctx, Collider2D other)
        {
            ctx.OnHit -= OnProjectileHit;
            // clean contaminant
            if (other.TryGetComponent<ICleanable>(out ICleanable cleanable))
                cleanable.Clean(cleanAmount);
            // stun before adding knockback
            if (other.TryGetComponent<IStunnable>(out IStunnable stunnable))
                stunnable.Stun(stunDuration);
            // add knockback
            if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                rb.AddForce((character.pointer.position - character.transform.position).normalized * knockback, ForceMode2D.Impulse);
        }
    }
}
