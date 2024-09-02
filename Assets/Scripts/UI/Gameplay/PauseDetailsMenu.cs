using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.LevelSelection.CharacterSelection;

namespace UI
{
    public class PauseDetailsMenu : MonoBehaviour
    {
        [Header("Menu Management")]
        [SerializeField] GameObject defaultPauseMenu;

        [Header("Character Details View")]
        [SerializeField] Image profileImage;
        [SerializeField] TextMeshProUGUI characterName;
        [SerializeField] TextMeshProUGUI characterDesc;

        [Header("Character Select Profile")]
        [SerializeField] Transform profileParent;
        [SerializeField] GameObject profilePrefab;

        List<ProfileObject> profileObjects = new List<ProfileObject>();
        class ProfileObject
        {
            public CharacterSelectProfile profile;
            public Button button;
        }

        PlayerCharacterSO[] characterInfo => GameManager.Instance.selectedCharacters;

        #region Handle Active
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
        #endregion

        #region Button Handlers
        public void ShowCharacter(CharacterSelectProfile ctx)
        {

        }
        #endregion

        #region Private Methods
        void LoadProfileObjects()
        {
            foreach (ProfileObject profileObject in profileObjects)
            {
                profileObject.profile?.gameObject.SetActive(false);
            }

            for (int i = 0; i < characterInfo.Length; i++)
            {
                CharacterSelectProfile selectedProfile;
                
                if (i >= profileObjects.Count)
                {
                    GameObject newObject = Instantiate(profilePrefab, profileParent);
                    ProfileObject newProfile = new ProfileObject 
                    {
                        profile = newObject.GetComponent<CharacterSelectProfile>(),
                        button = newObject.GetComponent<Button>()
                    };
                    profileObjects.Add(newProfile);
                    selectedProfile = profileObjects[^1].profile;
                }
                else 
                {
                    selectedProfile = profileObjects[i].profile;
                }

                if (selectedProfile == null) continue;
                selectedProfile.gameObject.SetActive(true);
                selectedProfile.SetCharacter(characterInfo[i]);
                selectedProfile.HideBorder();
            }
        }
        #endregion
    }
}
