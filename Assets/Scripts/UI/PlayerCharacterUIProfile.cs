using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.Animations;
using Entity.Data;
using Player;
using Behaviour = Player.Behaviours.Behaviour;
using UnityEngine.TextCore.Text;

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
        // caches
        Behaviour cacheCharacterBehaviour;
        PlayerCharacter cacheCharacterData;
        bool cacheCanTriggerSkill = false;

        // Start is called before the first frame update
        void Start()
        {
            // get UI animator component to play animations
            anim = GetComponent<UIAnimator>();
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
        /// <param name="characterManager">A reference to the player character manager</param>
        /// <param name="characterToShow">The character to be displayed on this icon</param>
        public void SetUI(CharacterManager characterManager, PlayerCharacter characterToShow)
        {
            // cache player character behaviour
            cacheCharacterBehaviour = characterToShow.GetComponent<Behaviour>();
            // cache player character data
            cacheCharacterData = characterToShow;

            // ensure sprite of character is not null, and set profile image
            if (characterToShow.characterSprite != null)
                profileImage.sprite = characterToShow.characterSprite;
            // do a null check for text, active player UI have no text, no need to update
            if (switchText != null)
                switchText.text = (Array.IndexOf(characterManager.character_instances, characterToShow) + 1).ToString();
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
            // ensure player character data and unswitchable overlay is not null
            if (cacheCharacterData == null || unswitchableOverlay == null) return;
            // darken profile image if cannot switch into character
            unswitchableOverlay.SetActive(!cacheCharacterData.Switchable);
        }
    }
}
