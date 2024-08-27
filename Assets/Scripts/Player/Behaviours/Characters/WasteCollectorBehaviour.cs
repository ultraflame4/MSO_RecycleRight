using UnityEngine;
using Interfaces;
using NPC.Contaminant;
using Level;

namespace Player.Behaviours
{
    public class WasteCollectorBehaviour : Behaviour
    {
        [Header("Attack: Grab")]
        [SerializeField] float grabDamage = 2f;
        [SerializeField] float grabRange = 1.5f;
        [SerializeField] Vector2 grabOffset;
        [SerializeField] LayerMask hitMask;
        [SerializeField] GameObject grabEffect;
        [SerializeField] AudioClip grabSFX;

        [Header("Attack: Throw")]
        [SerializeField] float throwDamage = 3f;
        [SerializeField] float throwStunDuration = .75f;
        [SerializeField] float throwForce = 25f;
        [SerializeField] GameObject throwEffect;
        [SerializeField] AudioClip throwSFX;

        [Header("Skill")]
        [SerializeField] GameObject garbageTruckPrefab;
        [SerializeField] AudioClip skillTruckSFX;
        [SerializeField] AudioClip skillHonkSFX;
        [SerializeField] float skillCameraShakeDuration = 1.5f;


        #region Grab Attack Variables
        Collider2D hit;
        Animator anim;
        Vector2 grabPosition => (Vector2) character.transform.position + 
            ((grabOffset * (Vector3.right * (character.Data.renderer.flipX ? -1f : 1f))) + (grabOffset * Vector3.up));
        float originalMovementSpeed;
        bool flippedCanSkill, flippedCanSwitch = false;
        bool grabbed => hit != null;
        #endregion

        #region Inherited Methods
        public override void TriggerAttack()
        {
            base.TriggerAttack();
            // check whether to grab or throw enemy
            if (!grabbed) 
                Grab();
            else 
                Throw();
        }

        public override void TriggerSkill()
        {
            base.TriggerSkill();
            // do not run if truck prefab is not found
            if (garbageTruckPrefab == null) return;
            // ensure sprite is not flipped
            character.Data.renderer.flipX = false;
            // get horizontal size of screen
            float screenHorSize = (Camera.main.orthographicSize * 2) * (Screen.width / Screen.height);
            // get position of left screen edge
            Vector2 edgePosition = Camera.main.transform.position;
            edgePosition.x -= screenHorSize;
            // instantiate truck prefab
            Instantiate(garbageTruckPrefab, edgePosition, Quaternion.identity);
            // handle truck spawn effects
            SoundManager.Instance?.PlayOneShot(skillTruckSFX);
            SoundManager.Instance?.PlayOneShot(skillHonkSFX);
            LevelManager.Instance?.camera?.ShakeCamera(skillCameraShakeDuration);
        }
        #endregion

        #region Methods to Handle Attack
        void Grab()
        {
            // detect enemy
            hit = Physics2D.OverlapCircle(character.transform.position, grabRange, hitMask);
            // if nothing is detected around player, try finding enemies around pointer
            hit = grabbed ? hit : Physics2D.OverlapCircle(character.pointer.position, grabRange, hitMask);
            // update grab animation boolean
            anim.SetBool("Grabbed", grabbed);
            // check if something is hit
            if (!grabbed) return;

            // apply grab damage to hit enemy
            hit.GetComponent<IDamagable>()?.Damage(grabDamage);
            // spawn grab vfx
            SpawnVFX(grabEffect);
            // play grab sfx
            SoundManager.Instance?.PlayOneShot(grabSFX);

            // check if grabbed object can be grabbed
            IAmNotMovableByWilson movable = hit.GetComponent<IAmNotMovableByWilson>();
            // handle not grabbing object
            if (movable == null || movable.CanMove()) return;
            hit = null;
        }

        void Throw()
        {
            // ensure hit is not null, ensure something has been grabbed
            if (!grabbed) return;

            // apply throw damage to hit enemy
            hit.GetComponent<IDamagable>()?.Damage(throwDamage);
            // apply stun to allow knockback
            hit.GetComponent<IStunnable>()?.Stun(throwStunDuration);
            // throw the hit enemy
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
               rb.AddForce(
                    (character.pointer.position - character.transform.position).normalized * 
                    throwForce, ForceMode2D.Impulse);
            }
            // spawn throw vfx
            SpawnVFX(throwEffect);
            // play throw sfx
            SoundManager.Instance?.PlayOneShot(throwSFX);
            // reset grab
            ResetGrab();
        }

        void ResetGrab()
        {
            // reset anything that was flipped
            if (flippedCanSkill) 
                canTriggerSkill = true;
            if (flippedCanSwitch) 
                character.CharacterManager.CanSwitchCharacters = true;
            // reset flip
            flippedCanSkill = false;
            flippedCanSwitch = false;

            // reset movement speed
            character.Data.movementSpeed = originalMovementSpeed;
            // after throw, reset hit to null
            hit = null;
            // update grab animation boolean
            anim.SetBool("Grabbed", grabbed);
        }

        void SpawnVFX(GameObject effect)
        {
            // ensure effects prefab is provided
            if (effect == null) return;
            // spawn hit vfx
            GameObject vfx = Instantiate(
                effect, 
                character.pointer.position, 
                Quaternion.identity, 
                character.transform
            );
            // set vfx direction
            vfx.transform.up = character.pointer.up;
        }
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            // get reference to animator
            anim = GetComponentInChildren<Animator>();
            // set original movement speed
            originalMovementSpeed = data.movementSpeed;
        }

        void Update()
        {
            // if is enabled and not grabbing, reset grab
            if (data != null && data.Enabled && !grabbed) ResetGrab();
            // do not run if nothing is grabbed
            if (!grabbed) return;
            // ensure cannot use skill when grabbed something
            if (canTriggerSkill)
            {
                canTriggerSkill = false;
                flippedCanSkill = true;
            }
            // ensure cannot switch character when grabbed something
            if (character.CharacterManager.CanSwitchCharacters)
            {
                character.CharacterManager.CanSwitchCharacters = false;
                flippedCanSwitch = true;
            }
        }

        void FixedUpdate()
        {
            // do not run if nothing is grabbed, or grabbed contaminant is dead
            if (!grabbed || hit.GetComponent<ContaminantNPC>()?.healthbar.value <= 0f) return;
            // lock position of hit enemy
            hit.transform.position = grabPosition;
            // set character movement speed to 0 to prevent movement
            data.movementSpeed = 0f;
        }
        #endregion

        protected void OnDrawGizmosSelected()
        {
            // ensure character is not null
            if (character == null) character = GetComponentInParent<PlayerController>();
            // if character cannot be found, do not draw gizmos
            if (character == null) return;
            // show grab range
            Gizmos.DrawWireSphere(character.pointer.position, grabRange);
            Gizmos.DrawWireSphere(character.transform.position, grabRange);
        }
    }
}
