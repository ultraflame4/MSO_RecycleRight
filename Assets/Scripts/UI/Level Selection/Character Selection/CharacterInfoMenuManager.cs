using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterInfoMenuManager : MonoBehaviour
    {
        [SerializeField] Image profileImage, characterImage;
        [SerializeField] TextMeshProUGUI characterName, description, desciptionTitle, errorText;

        void Start()
        {
            SetUIActive(false);
        }

        public void SetCharacter(PlayerCharacterSO characterData)
        {
            if (characterData == null) 
            {
                SetUIActive(false);
                return;
            }
            SetUIActive(true);
            characterName.text = characterData.characterName;
            description.text = characterData.characterDesc;
            profileImage.sprite = characterData.characterSprite;
            characterImage.sprite = characterData.prefab.GetComponentInChildren<SpriteRenderer>()?.sprite;
            profileImage.SetNativeSize();
            characterImage.SetNativeSize();
        }

        void SetUIActive(bool active)
        {
            errorText.gameObject.SetActive(!active);
            characterName.gameObject.SetActive(active);
            desciptionTitle.gameObject.SetActive(active);
            description.gameObject.SetActive(active);
            characterImage.gameObject.SetActive(active);
            profileImage.transform.parent.parent.gameObject.SetActive(active);
        }
    }
}
