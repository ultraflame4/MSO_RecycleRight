using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterUIProfile : MonoBehaviour
{
    [SerializeField] Image profileImage;
    [SerializeField] Image healthBar;
    [SerializeField] Text switchText;

    // reference animator component
    Animator anim;
    // caches
    PlayerController cachePlayer;
    bool cacheCanTriggerSkill = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSkillReadyAnimation();
    }

    /// <summary>
    /// Method to be called to set the image, health and text on the UI icon
    /// </summary>
    /// <param name="player">A reference to the player of type PlayerController</param>
    /// <param name="characterToShow">The character to be displayed on this icon</param>
    public void SetUI(PlayerController player, PlayerCharacter characterToShow)
    {
        // cache player
        cachePlayer = player;
        // check if skill is ready and update animation
        CheckSkillReadyAnimation();

        // ensure sprite of character is not null
        if (characterToShow.characterSprite != null)
            // set profile image
            profileImage.sprite = characterToShow.characterSprite;
        // do a null check for text, active player UI have no text, no need to update
        if (switchText != null)
            switchText.text = (Array.IndexOf(player.CharacterManager.character_instances, characterToShow) + 1).ToString();
        
        // set health
        // todo: health system
        float tempHealth = 0.75f * characterToShow.maxHealth;
        healthBar.fillAmount = tempHealth / characterToShow.maxHealth;
    }

    void CheckSkillReadyAnimation()
    {
        // do not run if animator is null
        if (anim == null) return;
        // ensure player is not null
        if (cachePlayer == null) return;
        // check if can trigger skill has changed
        if (cacheCanTriggerSkill == cachePlayer.CharacterBehaviour.CanTriggerSkill) return;
        // if so, update cache
        cacheCanTriggerSkill = cachePlayer.CharacterBehaviour.CanTriggerSkill;
        // play animation depending on if skill can currently be triggered
        anim.Play(cacheCanTriggerSkill ? "OnFire" : "Static");
    }
}
