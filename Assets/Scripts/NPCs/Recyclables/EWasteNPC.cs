using System.Collections;
using UnityEngine;

namespace NPC.Recyclable
{
    public class EWasteNPC : RecyclableNPC
    {

        public int startSmokeParticleCount = 2;
        public int endSmokeParticleCount = 20;
        public ParticleSystem smokeParticles;
        public SpriteRenderer fireSpriteR;
        public ParticleSystem fireParticles;
        public SpriteMask mask;

        [Tooltip("Once on fire, how long does it take to destroy the NPC?")]
        public float timeToDestroy = 10f;
        [Tooltip("Once on destroyed, how long does it take to disintegrate the NPC?")]
        public float timeToDisintegrate = 1f;
        private float fire_progress = 0;
        private float disintegrate_progress = 0;
        bool onFire => fire_progress > 0f;

        Coroutine fireCoroutine;

        protected override void Start()
        {
            base.Start();
            mask.alphaCutoff = 0;
            fireSpriteR.enabled = false;
        }

        public override void Contaminate(float dmg)
        {
            // EWasteNPCs are immune to contamination, but can be damaged.
            Damage(0);
        }


        public override void Damage(float damage)
        {
            SetOnFire();
        }

        [EasyButtons.Button]
        private void SetOnFire()
        {
            if (!Application.isPlaying) return;
            if (onFire) return;
            if (fireCoroutine != null) return;
            fireCoroutine = StartCoroutine(FireDamageProgress_Coroutine());
            smokeParticles.Stop();
            smokeParticles.Play();
        }


        IEnumerator FireDamageProgress_Coroutine()
        {
            bool startedFire = false;
            while (fire_progress < 1)
            {
                var emission = smokeParticles.emission;
                emission.rateOverTime = Mathf.Lerp(startSmokeParticleCount, endSmokeParticleCount, fire_progress);
                fire_progress += Time.deltaTime / timeToDestroy;

                if (fire_progress > 0.25f)
                {
                    // https://discussions.unity.com/t/particlesystem-play-does-not-play-particle/78698/3
                    // Particle systems stops emitting particles when play is called while it is already playing.
                    if (!startedFire)
                    {
                        startedFire = true;
                        fireParticles.Stop();
                        fireParticles.Play();
                    }
                }
                if (fire_progress > 0.5f)
                {
                    fireSpriteR.enabled = true;
                }
                yield return null;
            }
            fireSpriteR.enabled = false;

            smokeParticles.Stop();
            fireParticles.Stop();

            yield return null;
            // Stop all brains.
            state_Stunned.pause_timer=true;
            navigation.StopVelocity();
            SwitchState(state_Stunned);
            animator.enabled=false;
            // Queue explosion;

            yield return new WaitForSeconds(0.25f);
            while (disintegrate_progress < 1)
            {
                disintegrate_progress += Time.deltaTime / timeToDisintegrate;
                spriteRenderer.color = Color.black;
                mask.alphaCutoff = disintegrate_progress;
                yield return null;
            }
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }

    }
}