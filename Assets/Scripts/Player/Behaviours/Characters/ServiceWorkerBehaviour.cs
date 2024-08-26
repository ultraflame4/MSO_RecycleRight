using System.Linq;
using UnityEngine;
using Entity.Data;
using Interfaces;

namespace Player.Behaviours
{
    public class ServiceWorkerBehaviour : BaseMeleeAttack
    {
        [Header("Healing")]
        [SerializeField] float healAmount = 50f;
        [SerializeField, Range(0f, 1f)] float passiveHealScale = 0.25f;

        [Header("Effects")]
        [SerializeField] AudioClip attackSFX;
        [SerializeField] AudioClip passiveSFX;
        [SerializeField] AudioClip skillSFX;

        CharacterManager manager => character.CharacterManager;
        PlayerCharacter[] characters => manager.character_instances;
        float passiveHealAmount => healAmount * passiveHealScale;

        void Start()
        {
            // subscribe to on hit event
            OnHit += Passive;
            // start with skill active
            if (cooldown != null) StopCoroutine(cooldown);
            CooldownElasped = data.skillCooldown;
            canTriggerSkill = true;
            cooldown = null;
        }

        public override void TriggerAttack()
        {
            base.TriggerAttack();
            // play attack sfx
            SoundManager.Instance?.PlayOneShot(attackSFX);
        }

        public override void TriggerSkill()
        {
            if (manager == null)
            {
                Debug.LogWarning("Character manager could not be found! (ServiceWorkerSeraphineBehaviour.cs)");
                return;
            }

            if (characters == null || characters.Length <= 0) return;
            
            base.TriggerSkill();

            // play skill sfx
            SoundManager.Instance?.PlayOneShot(skillSFX);
            // apply healing to all characters
            foreach (PlayerCharacter character in characters)
            {
                Heal(healAmount, true, character);
            }
        }

        void Passive(Collider2D[] hits)
        {
            if (manager == null)
            {
                Debug.LogWarning("Character manager could not be found! (ServiceWorkerSeraphineBehaviour.cs)");
                return;
            }

            if (characters == null || characters.Length <= 0 || hits == null) return;

            // select character with lowest health
            PlayerCharacter selectedCharacter = characters
                .Where(x => x.Health > 0f)
                .OrderBy(x => x.Health)
                .ToArray()[0];
            // do not heal if character is at max health
            if (selectedCharacter.Health >= selectedCharacter.maxHealth) return;
            // sort characters array by health and heal lowest health character
            Heal(passiveHealAmount * hits.Length, false, selectedCharacter);
            // play passive sfx
            SoundManager.Instance?.PlayOneShot(passiveSFX);
        }

        void Heal(float amount, bool canRevive, PlayerCharacter target)
        {
            // check if target can be revived and already dead
            if (target.Health <= 0 && !canRevive) return;
            // try to get damagable component
            if (!target.TryGetComponent<IDamagable>(out IDamagable damagable)) return;
            // negetive damage == healing
            damagable.Damage(-amount);
        }
    }
}
