using UnityEngine;
using Interfaces;

namespace Player.Behaviours
{
    public class WasteCollectorBehaviour : Behaviour
    {
        [Header("Attack: Grab")]
        [SerializeField] float grabDamage = 2f;
        [SerializeField] float grabRange = 1.5f;
        [SerializeField] Vector3 grabOffset;
        [SerializeField] LayerMask hitMask;
        [SerializeField] GameObject grabEffect;

        [Header("Attack: Throw")]
        [SerializeField] float throwDamage = 3f;
        [SerializeField] float throwForce = 25f;
        [SerializeField] GameObject throwEffect;

        #region Grab Attack Variables
        Collider2D hit;
        Vector3 grabPosition;
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
        }
        #endregion

        #region Methods to Handle Attack
        void Grab()
        {
            // detect enemy
            hit = Physics2D.OverlapCircle(character.transform.position, grabRange, hitMask);
            // if nothing is detected around player, try finding enemies around pointer
            hit = grabbed ? hit : Physics2D.OverlapCircle(character.pointer.position, grabRange, hitMask);
            // check if something is hit
            if (!grabbed) return;

            // apply grab damage to hit enemy
            hit.GetComponent<IDamagable>()?.Damage(grabDamage);
            // cache original movement speed
            originalMovementSpeed = character.Data.movementSpeed;

            // set grab position of enemy, and apply offset based on which direction the player is facing
            grabPosition = character.transform.position + grabOffset;
            grabPosition.x *= character.Data.renderer.flipX ? -1f : 1f;
            // reset flip
            flippedCanSkill = false;
            flippedCanSwitch = false;
        }

        void Throw()
        {
            // ensure hit is not null, ensure something has been grabbed
            if (!grabbed) return;

            // apply throw damage to hit enemy
            hit.GetComponent<IDamagable>()?.Damage(throwDamage);
            // throw the hit enemy
            hit.GetComponent<Rigidbody2D>()?.AddForce(
                (character.pointer.position - character.transform.position).normalized * 
                throwForce, ForceMode2D.Impulse);

            // reset anything that was flipped
            if (flippedCanSkill) 
                canTriggerSkill = true;
            if (flippedCanSwitch) 
                character.CharacterManager.CanSwitchCharacters = true;

            // reset movement speed
            character.Data.movementSpeed = originalMovementSpeed;
            // after throw, reset hit to null
            hit = null;
        }
        #endregion

        #region MonoBehaviour Callbacks
        void Update()
        {
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
            // do not run if nothing is grabbed
            if (!grabbed) return;
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
