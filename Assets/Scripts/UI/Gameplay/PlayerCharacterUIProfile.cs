using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.Animations;
using Entity.Data;
using Player;
using Behaviour = Player.Behaviours.Behaviour;

namespace UI
{
    public class PlayerCharacterUIProfile : MonoBehaviour
    {
        [SerializeField] Image profileImage;
        [SerializeField] Image healthBar;
        [SerializeField] TextMeshProUGUI switchText;
        [SerializeField] GameObject unswitchableOverlay;

        // reference animator component
        UIAnimator anim;
        // reference character manager component
        CharacterManager characterManager => PlayerController.Instance.CharacterManager;

        // caches
        Behaviour cacheCharacterBehaviour;
        PlayerCharacter cacheCharacterData;
        Color cacheOriginalProfileColor;
        bool cacheCanTriggerSkill = false;

        // Start is called before the first frame update
        void Start()
        {
            // get UI animator component to play animations
            anim = GetComponent<UIAnimator>();
            
            // cache original profile image color
            cacheOriginalProfileColor = profileImage.color;
            // disable unswitchable overlay on start
            if (unswitchableOverlay == null) return; 
            unswitchableOverlay.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            // update UI
            CheckSwitchability();
            CheckSkillReadyAnimation();
            UpdateCharacterHealth();
        }

        /// <summary>
        /// Method to be called to set the image, health and text on the UI icon
        /// </summary>
        /// <param name="characterToShow">The character to be displayed on this icon</param>
        public void SetUI(PlayerCharacter characterToShow)
        {
            // ensure character manager is not null
            if (characterManager == null) {
                Debug.LogWarn("Character Manager is null. Skipping ui set!");
                return;
            }

            // cache player character behaviour
            cacheCharacterBehaviour = characterToShow.GetComponent<Behaviour>();
            // cache player character data
            cacheCharacterData = characterToShow;
            Debug.Log($"Setting UI for {characterToShow.characterName} for {gameObject.name}.");
            // do a null check for text, active player UI have no text, no need to update
            if (switchText != null)
                switchText.text = (Array.IndexOf(characterManager.character_instances, characterToShow) + 1).ToString();
            
            // ensure profile image is not null before attempting to set the sprite
            if (profileImage == null) return;
            // set profile image of character
            profileImage.sprite = characterToShow.characterSprite;
            // if no sprite is found, set default sprite, otherwise set color to white to show sprite
            profileImage.color = characterToShow.characterSprite == null ? cacheOriginalProfileColor : Color.white;
        }

        // methods to update UI
        void UpdateCharacterHealth()
        {
            // ensure player character data is not null
            if (cacheCharacterData == null) return;
            // update health bar
            healthBar.fillAmount = cacheCharacterData.Health / cacheCharacterData.maxHealth;
        }

        void CheckSkillReadyAnimation()
        {
            // do not run if animator is null
            if (anim == null) return;
            // ensure player behaviour is not null
            if (cacheCharacterBehaviour == null) return;
            // check if can trigger skill has changed
            if (cacheCanTriggerSkill == cacheCharacterBehaviour.CanTriggerSkill) return;
            // if so, update cache
            cacheCanTriggerSkill = cacheCharacterBehaviour.CanTriggerSkill;
            // play animation depending on if skill can currently be triggered
            anim.Play(cacheCanTriggerSkill ? "FireOn" : "Static");
        }

        void CheckSwitchability()
        {
            // ensure player character data, unswitchable overlay and character manager is not null
            if (cacheCharacterData == null || unswitchableOverlay == null || characterManager == null) return;
            // darken profile image if cannot switch into character
            unswitchableOverlay.SetActive(!cacheCharacterData.Switchable || !characterManager.CanSwitchCharacters);
        }
    }
}
