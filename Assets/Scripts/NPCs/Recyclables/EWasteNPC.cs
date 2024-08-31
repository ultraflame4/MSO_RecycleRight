using System.Collections;
using Interfaces;
using Level;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NPC.Recyclable
{
    public class EWasteNPC : RecyclableNPC, ICleanable
    {

        public int startSmokeParticleCount = 2;
        public int endSmokeParticleCount = 20;
        public ParticleSystem smokeParticles;
        public SpriteRenderer fireSpriteR;
        public ParticleSystem fireParticles;
        public ParticleSystem explodeParticles;
        public AudioSource fireAudioSrc; // only really used for burning sound.
        public AudioClip explosionClip; // only really used for explosion sound.
        public SpriteMask mask;

        [Header("EWaste Settings")]
        [Tooltip("Once on fire, how long does it take to destroy the NPC?")]
        public float timeToDestroy = 10f;
        [Tooltip("Once on destroyed, how long does it take to disintegrate the NPC?")]
        public float timeToDisintegrate = 1f;
        [Tooltip("The radius to set tiles on fire.")]
        public float explosionFireRadius = 1f;
        [Tooltip("The prefab for fire tiles."), SerializeField]
        private GameObject fireTilePrefab;



        private float fire_progress = 0;
        private float disintegrate_progress = 0;
        bool onFire => fire_progress > 0f;

        public bool AllowCleanable => true;

        Coroutine fireCoroutine;

        protected override void Start()
        {
            base.Start();
            mask.alphaCutoff = 0;
            fireSpriteR.enabled = false;
            fireAudioSrc.Stop();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionFireRadius);
        }

        public override void Contaminate(float dmg)
        {
            // EWasteNPCs are immune to contamination, but can be damaged.
            SetOnFire();
        }


        public override void Damage(float damage)
        {
            // SetOnFire();
        }
        // EWaste cannot be cleaned hence cleaning it will break stuff and set things on fire.
        public void Clean(float clean_amount)
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
            fireAudioSrc.Play();
        }

        [EasyButtons.Button]
        private void Explode()
        {
            if (!Application.isPlaying) return;
            if (!explodeParticles.isPlaying)
            {
                explodeParticles.Play();
            }
            var tilemap = GameObject.FindWithTag("FireTilemap")?.GetComponent<Tilemap>();
            if (tilemap == null)
            {
                Debug.LogWarning("No fire tilemap found. No tiles will be set on fire.");
                return;
            }

            // Calculate the square bounds of the explosion. (min and max)
            var min = tilemap.WorldToCell(new Vector3(
                transform.position.x - explosionFireRadius,
                transform.position.y - explosionFireRadius,
                transform.position.z
                ));

            var max = tilemap.WorldToCell(new Vector3(
                transform.position.x + explosionFireRadius,
                transform.position.y + explosionFireRadius,
                transform.position.z
            ));

            // Loop through the bounds and set the tiles on fire.
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    var pos = new Vector3Int(x, y, min.z);
                    // Check if the distance is within the radius. needed because the bounds are square.
                    if (Vector3.Distance(tilemap.GetCellCenterWorld(pos), transform.position) > explosionFireRadius)
                    {
                        continue;
                    }
                    var a = ScriptableObject.CreateInstance<Tile>();
                    a.gameObject = fireTilePrefab;
                    var fireTile = a.gameObject.GetComponent<FireTile>();
                    fireTile.associatedTilemap = tilemap;
                    tilemap.SetTile(pos, a);
                }
            }

            fireAudioSrc.Stop();
            if (SoundManager.hasInstance){
                SoundManager.Instance.PlayOneShot(explosionClip);
                LevelManager._instance?.camera?.ShakeCamera(0.1f);
            }

        }

        IEnumerator FireDamageProgress_Coroutine()
        {
            while (fire_progress < 1)
            {
                var emission = smokeParticles.emission;
                emission.rateOverTime = Mathf.Lerp(startSmokeParticleCount, endSmokeParticleCount, fire_progress);
                fire_progress += Time.deltaTime / timeToDestroy;

                if (fire_progress > 0.25f)
                {
                    // https://discussions.unity.com/t/particlesystem-play-does-not-play-particle/78698/3
                    // Particle systems stops emitting particles when play is called while it is already playing.
                    if (!fireParticles.isPlaying)
                    {
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
            state_Stunned.pause_timer = true;
            navigation.StopVelocity();
            SwitchState(state_Stunned);
            animator.enabled = false;
            // Queue explosion;
            Explode();
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