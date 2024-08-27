using UnityEngine;
using Entity.Data;
using Level;

namespace Player.Behaviours
{
    public class VolunteerBehaviour : BaseMeleeAttack
    {
        [Header("Passive")]
        [SerializeField, Range(0f, 1f)] float passiveChance = 0.45f;

        [Header("Skill")]
        [SerializeField] float buffScale = 1.5f;
        [SerializeField] float buffDuration = 15f;

        [Header("Effects")]
        [SerializeField] AudioClip attackSFX;
        [SerializeField] AudioClip attackStrongSFX;
        [SerializeField] AudioClip skillSFX;
        [SerializeField] GameObject skillVFX;
        GameObject vfx_prefab;

        public override void TriggerAttack()
        {
            // boolean to check if passive is triggered
            bool passiveTriggered = false;
            // check for passive
            if (Random.Range(0f, 1f) <= passiveChance)
            {
                // set passive triggered to true
                passiveTriggered = true;
                // double damage and knockback
                attackDamage *= 2;
                knockback *= 2;
                // play effects to show a critical hit
                SoundManager.Instance?.PlayOneShot(attackStrongSFX);
                LevelManager.Instance?.camera?.ShakeCamera(0.15f);
            }
            else 
            {
                // play effects for normal attack (no crit)
                SoundManager.Instance?.PlayOneShot(attackSFX);
            }
            // trigger base attack
            base.TriggerAttack();
            // check if passive is triggered, if so, revert changes
            if (!passiveTriggered) return;
            // revert damage and knockback
            attackDamage /= 2;
            knockback /= 2;
        }

        public override void TriggerSkill()
        {
            base.TriggerSkill();
            // apply buff
            SetBuffActive(true);
            // start coroutine to count buff duration
            StartCoroutine(CountDuration(buffDuration, () => SetBuffActive(false)));
            // play sfx
            SoundManager.Instance?.PlayOneShot(skillSFX);
            // create vfx prefab
            vfx_prefab = Instantiate(skillVFX, transform.position, Quaternion.identity, transform.parent.parent);
        }

        // private methods
        void SetBuffActive(bool active)
        {
            // apply attack speed increase to all characters by looping through all characters
            foreach (PlayerCharacter otherCharaData in character.CharacterManager.character_instances)
            {
                Animator otherCharaAnim = otherCharaData.GetComponentInChildren<Animator>();
                if (otherCharaAnim == null) continue;
                otherCharaAnim.speed = active ? buffScale : 1f;
                otherCharaData.movementMultiplier = active ? buffScale : 1f;
                otherCharaData.attackMultiplier = active ? 1f / buffScale : 1f;
            }

            // destroy vfx prefab after skill duration
            if (active || vfx_prefab == null) return;
            Destroy(vfx_prefab);
            vfx_prefab = null;
        }
    }
}
