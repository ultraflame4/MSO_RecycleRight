using System.Collections;
using System.Collections.Generic;
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

        // skill variables to cache current active character components
        PlayerCharacter cacheActiveData;
        Animator cacheActiveAnimator;

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
            // set animation speed
            character.anim.speed = buffScale;
            // set animation duration
            data.attackDuration *= 1 / buffDuration;
            // cache current active player
            cacheActiveData = data;
            cacheActiveAnimator = character.anim;
            // subscribe to characcter change event
            character.CharacterManager.CharacterChanged += OnCharacterChange;
            // start coroutine to count buff duration
            StartCoroutine(CountBuffDuration(buffDuration));
        }

        IEnumerator CountBuffDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            // reset animation speed
            cacheActiveAnimator.speed = 1f;
            // reset animation duration
            cacheActiveData.attackDuration *= buffDuration;
            // unsubscribe to character change event
            character.CharacterManager.CharacterChanged -= OnCharacterChange;
        }

        // private methods
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

        // event listener to transfer changes to animator speed to new character
        void OnCharacterChange(PlayerCharacter data)
        {
            // reset buffs on previous character
            cacheActiveAnimator.speed = 1f;
            cacheActiveData.attackDuration *= buffDuration;
            // apply buffs to new character
            character.anim.speed = buffScale;
            data.attackDuration *= 1 / buffDuration;
            // cache new character
            cacheActiveAnimator = character.anim;
            cacheActiveData = data;
        }
    }
}
