using UnityEngine;
using Entity.Data;

namespace Player.Behaviours
{
    public class VolunteerBehaviour : BaseMeleeAttack
    {
        [Header("Passive")]
        [SerializeField, Range(0f, 1f)] float passiveChance = 0.45f;

        [Header("Skill")]
        [SerializeField] float buffScale = 1.5f;
        [SerializeField] float buffDuration = 15f;

        [Header("VFX")]
        [SerializeField] GameObject hitEffects;

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
                // todo: add some effect to indicate this has been triggered
            }
            // spawn vfx
            SpawnVFX();
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
        }

        // private methods
        void SetBuffActive(bool active)
        {
            // apply attack speed increase to all characters by looping through all characters
            foreach (PlayerCharacter otherCharaData in character.CharacterManager.character_instances)
            {
                Animator otherCharaAnim = otherCharaData.GetComponent<Animator>();
                if (otherCharaAnim == null) continue;
                otherCharaAnim.speed = active ? buffScale : 1f;
                otherCharaData.attackDuration *= active ? 1 / buffScale : buffScale;
                otherCharaData.movementSpeed *= active ? buffScale : 1 / buffScale;
            }
        }

        void SpawnVFX()
        {
            // ensure hit effects prefab is provided
            if (hitEffects == null) return;
            // spawn hit vfx
            GameObject vfx = Instantiate(
                hitEffects, 
                character.pointer.position, 
                Quaternion.identity, 
                character.transform
            );
            // set vfx direction
            vfx.transform.up = character.pointer.up;
        }
    }
}
