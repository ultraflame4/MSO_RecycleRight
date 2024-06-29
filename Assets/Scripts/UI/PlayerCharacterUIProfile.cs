using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterUIProfile : MonoBehaviour
{
    [SerializeField] Image profileImage;
    [SerializeField] Image healthBar;
    [SerializeField] Text switchText;

    /// <summary>
    /// Method to be called to set the image, health and text on the UI icon
    /// </summary>
    /// <param name="player">A reference to the player of type PlayerController</param>
    /// <param name="characterToShow">The character to be displayed on this icon</param>
    public void SetUI(PlayerController player, PlayerCharacter characterToShow)
    {
        // set profile image
        profileImage.sprite = characterToShow.characterSprite;
        // set health
        // todo: health system
        float tempHealth = 0.5f * characterToShow.maxHealth;
        healthBar.fillAmount = tempHealth / characterToShow.maxHealth;
        // do a null check for text, active player UI have no text, no need to update
        if (switchText == null) return;
        switchText.text = (Array.IndexOf(player.CharacterManager.character_instances, characterToShow) + 1).ToString();
    }
}
