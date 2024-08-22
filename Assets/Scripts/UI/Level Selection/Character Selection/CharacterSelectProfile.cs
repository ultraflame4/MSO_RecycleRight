using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterSelectProfile : MonoBehaviour
    {
        [SerializeField] Image image, border, selection;
        [SerializeField] TextMeshProUGUI selectText;
        public PlayerCharacterSO currentCharacter { get; private set; }

        public event Action<CharacterSelectProfile> CharacterSelected;

        /// <summary>
        /// Set the character stored in this profile icon
        /// </summary>
        /// <param name="characterData">Data of character to set</param>
        public void SetCharacter(PlayerCharacterSO characterData)
        {
            currentCharacter = characterData;
            if (image == null) return;
            image.sprite = currentCharacter.characterSprite;
            image.SetNativeSize();
        }

        /// <summary>
        /// Set active of the border indicator for if profile is selected
        /// </summary>
        /// <param name="select">Selection indicator active state</param>
        public void SetSelection(bool select)
        {
            selection?.gameObject.SetActive(select);
        }

        /// <summary>
        /// Select profile icon
        /// </summary>
        /// <param name="color">Color of border</param>
        /// <param name="index">Index of character in party</param>
        public void ShowBorder(Color color, int index)
        {
            border?.gameObject.SetActive(true);
            border.color = color;
            selectText.text = $"P{index + 1}";
        }

        /// <summary>
        /// Deselect profile icon
        /// </summary>
        public void HideBorder()
        {
            border?.gameObject.SetActive(false);
        }

        /// <summary>
        /// Invoke event to select current character
        /// </summary>
        public void SelectCharacter()
        {
            CharacterSelected?.Invoke(this);
        }
    }
}
