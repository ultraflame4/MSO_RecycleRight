using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterSelectProfile : MonoBehaviour
    {
        [SerializeField] Image image;
        public PlayerCharacterSO currentCharacter { get; private set; }

        public void SetCharacter(PlayerCharacterSO characterData)
        {
            currentCharacter = characterData;
            if (image == null) return;
            image.sprite = currentCharacter.characterSprite;
            image.SetNativeSize();
        }
    }
}
