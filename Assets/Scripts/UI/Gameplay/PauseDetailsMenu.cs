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
        [SerializeField] TextMeshProUGUI characterName, characterDesc;
        [SerializeField] CharacterSelectProfile[] characterProfiles;

        [Header("Character Tags")]
        [SerializeField] TextMeshProUGUI attackType;
        [SerializeField] TextMeshProUGUI attackTarget;
        [SerializeField] TextMeshProUGUI role1, role2, role3;

        PlayerCharacter[] characterInstance => PlayerController.Instance.CharacterManager.character_instances;
        PlayerCharacterSO[] characterInfo => GameManager.Instance.selectedCharacters;
        PlayerCharacterSO currentCharacter;

        void Update()
        {
            // allow using number keys to swithc profile
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ShowCharacter(characterProfiles[0]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ShowCharacter(characterProfiles[1]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ShowCharacter(characterProfiles[2]);
            }
        }

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
            // do not set data if profile is deactivated
            if (!ctx.gameObject.activeSelf) return;
            // do not set if current set character is the same as the character to set
            if (currentCharacter != null && currentCharacter == ctx.currentCharacter) return;

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

            attackType.text = ctx.currentCharacter.attackType.ToString();
            attackTarget.text = ctx.currentCharacter.attackTarget.ToString().Replace("_", "-");
            SetCharacterRole(ctx.currentCharacter.role1, role1);
            SetCharacterRole(ctx.currentCharacter.role2, role2);
            SetCharacterRole(ctx.currentCharacter.role3, role3);

            currentCharacter = ctx.currentCharacter;
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
                if (characterProfiles.Length < characterInfo.Length)
                {
                    Debug.LogWarning("There are not enough character profiles set! Loading of character data is aborted! (PauseDetailsMenu.cs)");
                    return;
                }

                if (i >= characterInstance.Length)
                {
                    characterProfiles[i].gameObject.SetActive(false);
                    continue;
                }

                characterProfiles[i].gameObject.SetActive(true);
                characterProfiles[i].HideBorder();
                characterProfiles[i].SetCharacter(characterInfo[i]);
                characterProfiles[i].SetSelection(characterInstance[i].Enabled);
                if (characterInstance[i].Enabled) ShowCharacter(characterProfiles[i]);
            }
        }

        void SetCharacterRole(CharacterRoles role, TextMeshProUGUI targetUI)
        {
            // if role is none, set toe string to blank
            // replace underscore with space
            string roleString = role == CharacterRoles.NONE ? "" : role.ToString().Replace("_", " ");
            targetUI.text = roleString;
        }
        #endregion
    }
}
