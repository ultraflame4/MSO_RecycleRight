using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterInfoMenuManager : MonoBehaviour
    {
        [Header("Basic UI")]
        [SerializeField] Image profileImage;
        [SerializeField] TextMeshProUGUI characterName, description, desciptionTitle, errorText;

        [Header("Character Tags UI")]
        [SerializeField] TextMeshProUGUI attackType;
        [SerializeField] TextMeshProUGUI attackTarget;
        [SerializeField] TextMeshProUGUI role1, role2, role3;


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
            profileImage.SetNativeSize();

            attackType.text = characterData.attackType.ToString();
            attackTarget.text = characterData.attackTarget.ToString().Replace("_", "-");
            SetCharacterRole(characterData.role1, role1);
            SetCharacterRole(characterData.role2, role2);
            SetCharacterRole(characterData.role3, role3);
        }

        void SetCharacterRole(CharacterRoles role, TextMeshProUGUI targetUI)
        {
            // if role is none, set toe string to blank
            // replace underscore with space
            string roleString = role == CharacterRoles.NONE ? "" : role.ToString().Replace("_", " ");
            targetUI.text = roleString;
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
            profileImage.transform.parent.parent.gameObject.SetActive(active);

            attackType.gameObject.SetActive(active);
            attackTarget.gameObject.SetActive(active);
            role1.gameObject.SetActive(active);
            role2.gameObject.SetActive(active);
            role3.gameObject.SetActive(active);
        }
    }
}
