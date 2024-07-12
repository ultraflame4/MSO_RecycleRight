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
        CharacterManager characterManager;
        float[] cooldownManager;
        int activeIndex;

        // Start is called before the first frame update
        void Start()
        {
            // get reference to character manager
            characterManager = GameObject.FindWithTag("Player").GetComponent<CharacterManager>();
            // ensure character manager is not null
            if (characterManager == null) return;
            // reset active index to 0
            activeIndex = 0;
             // set character skill icon of first character, ensure icon image is not null
            if (iconImage != null) iconImage.sprite = characterManager.character_instances[activeIndex].skillIcon;
            // subscribe to skill triggered event
            Behaviour firstCharacterBehaviour = characterManager.character_instances[activeIndex].GetComponent<Behaviour>();
            if (firstCharacterBehaviour != null) firstCharacterBehaviour.SkillTriggered += OnSkillTrigger;
            // subscribe to character change event
            characterManager.CharacterChanged += OnCharacterChange;
            // set cooldown manager array to calculate cooldown of all characters
            SetCooldownManager();
        }

        // Update is called once per frame
        void Update()
        {
            // handle cooldown
            for (int i = 0; i < cooldownManager.Length; i++)
            {
                // only increment cooldown when less than max cooldown
                if (cooldownManager[i] >= characterManager.character_instances[i].netSkillCooldown) 
                        continue;
                // increment cooldown
                cooldownManager[i] += Time.deltaTime;
            }
            // update cooldown overlay
            cooldownOverlay.fillAmount = 1f - (cooldownManager[activeIndex] / 
                (characterManager.character_instances[activeIndex].skillCooldown * 
                characterManager.character_instances[activeIndex].skillCooldownMultiplier));
        }

        void SetCooldownManager()
        {
            // ensure character manager is not null
            if (characterManager == null) return;
            // create a new array the length of the number of characters
            cooldownManager = new float[characterManager.character_instances.Length];
            // reset all values to 0f and false respectively
            for (int i = 0; i < cooldownManager.Length; i++)
            {
                cooldownManager[i] = 0f;
            }
        }

        // event listeners
        void OnCharacterChange(PlayerCharacter prev, PlayerCharacter curr)
        {
            // update active index
            activeIndex = Array.IndexOf(characterManager.character_instances, curr);
            // set character skill icon, ensure icon image is not null
            if (iconImage != null) iconImage.sprite = curr.skillIcon;
            // get behaviour of new character
            Behaviour characterBehaviour = curr.GetComponent<Behaviour>();
            // subscribe from skill triggered event of new character if behaviour is not null
            if (characterBehaviour != null) characterBehaviour.SkillTriggered += OnSkillTrigger;
            // ensure previous character is not null
            if (prev == null) return;
            // get behaviour of previous character
            characterBehaviour = prev.GetComponent<Behaviour>();
            // unsubscribe from skill triggered event of previous character if behaviour is not null
            if (characterBehaviour != null) characterBehaviour.SkillTriggered -= OnSkillTrigger;
        }

        void OnSkillTrigger(PlayerCharacter character)
        {
            // reset cooldown for active character whenever skill is triggered
            // set to negetive skill duration to offset skill duration
            cooldownManager[activeIndex] = -characterManager.character_instances[activeIndex].skillDuration;
        }
    }
}
