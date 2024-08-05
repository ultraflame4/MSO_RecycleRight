using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterInfoMenuManager : MonoBehaviour
    {
        [SerializeField] Image profileImage, characterImage;
        [SerializeField] TextMeshProUGUI characterName, description, desciptionTitle, errorText;
        public PlayerCharacterSO selectedCharacter { get; private set; }

        void Start()
        {
            SetUI();
        }

        /// <summary>
        /// Set the data of the character to display
        /// </summary>
        /// <param name="characterData">Character data to be displayed</param>
        public void SetCharacter(PlayerCharacterSO characterData)
        {
            selectedCharacter = characterData;

            if (characterData == null) return;

            characterName.text = characterData.characterName;
            description.text = characterData.characterDesc;
            profileImage.sprite = characterData.characterSprite;
            characterImage.sprite = characterData.prefab.GetComponentInChildren<SpriteRenderer>()?.sprite;
            profileImage.SetNativeSize();
            characterImage.SetNativeSize();
        }

        /// <summary>
        /// Set whether to display character information or error message
        /// </summary>
        public void SetUI()
        {
            bool active = selectedCharacter != null;
            errorText.gameObject.SetActive(!active);
            characterName.gameObject.SetActive(active);
            desciptionTitle.gameObject.SetActive(active);
            description.gameObject.SetActive(active);
            characterImage.gameObject.SetActive(active);
            profileImage.transform.parent.parent.gameObject.SetActive(active);
        }
    }
}
