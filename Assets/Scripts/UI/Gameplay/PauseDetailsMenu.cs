using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.LevelSelection.CharacterSelection;
using Entity.Data;
using Player;

namespace UI
{
    public class PauseDetailsMenu : MonoBehaviour
    {
        [Header("Menu Management")]
        [SerializeField] GameObject defaultPauseMenu;

        [Header("Character View")]
        [SerializeField] Image profileImage;
        [SerializeField] TextMeshProUGUI characterName;
        [SerializeField] TextMeshProUGUI characterDesc;
        [SerializeField] CharacterSelectProfile[] characterProfiles;

        PlayerCharacter[] characterInstance => PlayerController.Instance.CharacterManager.character_instances;
        PlayerCharacterSO[] characterInfo => GameManager.Instance.selectedCharacters;

        #region Button Handlers
        /// <summary>
        /// Show pause details menu
        /// </summary>
        public void Activate()
        {
            gameObject.SetActive(true);
            defaultPauseMenu.SetActive(false);
            LoadProfileObjects();
        }

        /// <summary>
        /// Hide pause details menu
        /// </summary>
        public void Deactivate()
        {
            gameObject.SetActive(false);
            defaultPauseMenu.SetActive(true);
        }

        public void ShowCharacter(CharacterSelectProfile ctx)
        {
            if (characterProfiles == null)
            {
                Debug.LogWarning("Character profiles array is not set in editor mode, try testing from level selection scene. (PauseDetailsMenu.cs)");
                return;
            }

            foreach (CharacterSelectProfile profile in characterProfiles)
            {
                profile.SetSelection(profile == ctx);
            }

            // set data
            profileImage.sprite = ctx.currentCharacter.characterSprite;
            characterName.text = ctx.currentCharacter.characterName;
            characterDesc.text = ctx.currentCharacter.characterDesc;
            characterDesc.transform.position = new Vector2(characterDesc.transform.position.x, 0f);
        }
        #endregion

        #region Private Methods
        void LoadProfileObjects()
        {
            if (characterProfiles == null)
            {
                Debug.LogWarning("Character profiles array is not set in editor mode, try testing from level selection scene. (PauseDetailsMenu.cs)");
                return;
            }

            for (int i = 0; i < characterProfiles.Length; i++)
            {
                if (i >= characterInfo.Length)
                {
                    Debug.LogWarning("There are not enough character profiles set! Loading of character data is aborted! (PauseDetailsMenu.cs)");
                    return;
                }

                characterProfiles[i].SetCharacter(characterInfo[i]);
                characterProfiles[i].HideBorder();

                if (i >= characterInstance.Length)
                {
                    characterProfiles[i].gameObject.SetActive(false);
                }
                else 
                {
                    characterProfiles[i].gameObject.SetActive(true);
                    characterProfiles[i].SetSelection(characterInstance[i].Enabled);
                    if (characterInstance[i].Enabled) ShowCharacter(characterProfiles[i]);
                }
            }
        }
        #endregion
    }
}
