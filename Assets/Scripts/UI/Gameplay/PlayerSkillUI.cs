using System;
using UnityEngine;
using UnityEngine.UI;
using Entity.Data;
using Player;
using Behaviour = Player.Behaviours.Behaviour;

namespace UI
{
    public class PlayerSkillUI : MonoBehaviour
    {
        [SerializeField] Image iconImage;
        [SerializeField] Image cooldownOverlay;
        CharacterManager characterManager => PlayerController.Instance.CharacterManager;
        Behaviour[] behaviours;
        int activeIndex;

        // Start is called before the first frame update
        void Start()
        {
            // ensure character manager is not null
            if (characterManager == null) return;
            // reset active index to 0
            activeIndex = 0;
             // set character skill icon of first character, ensure icon image is not null
            if (iconImage != null) iconImage.sprite = characterManager.character_instances[activeIndex].skillIcon;
            // store all character behaviours
            behaviours = new Behaviour[characterManager.character_instances.Length];
            for (int i = 0; i < characterManager.character_instances.Length; i++)
            {
                behaviours[i] = characterManager.character_instances[i].GetComponent<Behaviour>();
            }
            // subscribe to character change event
            characterManager.CharacterChanged += OnCharacterChange;
        }

        // Update is called once per frame
        void Update()
        {
            // update cooldown overlay
            cooldownOverlay.fillAmount = 1f - (behaviours[activeIndex].CooldownElasped / 
                characterManager.character_instances[activeIndex].skillCooldown);
        }

        /// <summary>
        /// Externally trigger the player's skill
        /// </summary>
        public void TriggerSkill()
        {
            PlayerController.Instance?.DefaultState?.ExternalTriggerSkill();
        }

        // event listeners
        void OnCharacterChange(PlayerCharacter prev, PlayerCharacter curr)
        {
            // do not switch to character if character cannot be switched into
            if (!curr.Switchable) return;
            // update active index
            activeIndex = Array.IndexOf(characterManager.character_instances, curr);
            // set character skill icon, ensure icon image is not null
            if (iconImage != null) iconImage.sprite = curr.skillIcon;
        }
    }
}
