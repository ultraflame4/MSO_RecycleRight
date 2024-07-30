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

        [Tooltip("Once on fire, how long does it take to destroy the NPC?")]
        public float timeToDestroy = 10f;
        private float progress = 0;
        bool onFire => progress > 0f;

        Coroutine fireCoroutine;

        protected override void Start() {
            base.Start();
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
            while (progress < 1)
            {
                var emission = smokeParticles.emission;
                emission.rateOverTime = Mathf.Lerp(startSmokeParticleCount, endSmokeParticleCount, progress);
                progress += Time.deltaTime / timeToDestroy;

                if (progress > 0.25f)
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
                if (progress > 0.5f)
                {
                    fireSpriteR.enabled = true;
                }
                yield return null;
            }
            Destroy(gameObject);
        }

    }
}