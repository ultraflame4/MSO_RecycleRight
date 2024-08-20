using UnityEngine;
using Entity.Data;
using Player;

namespace UI
{
    public class PlayerCharacterUIProfileManager : MonoBehaviour
    {
        [SerializeField] PlayerCharacterUIProfile[] UIIcons;
        CharacterManager characterManager => PlayerController.Instance.CharacterManager;
        bool started = false;

        // Start is called before the first frame update
        void Start()
        {
            // ensure character manager and character instance is not null
            if (characterManager == null || characterManager.character_instances == null || 
                characterManager.character_instances.Length <= 0) 
                    return;
            // subscribe to character change event
            characterManager.CharacterChanged += OnCharacterChange;
            // start by setting all UI icons
            SetAllUI(characterManager.character_instances[0]);
            // set boolean managing start to true
            started = true;
        }

        // Update is called once per frame
        void Update()
        {
            // ensure character manager and character instance is not null
            if (characterManager == null || characterManager.character_instances == null || 
                characterManager.character_instances.Length <= 0) 
                    return;
            // when not null, check whether started, if not, call start again
            else if (!started)
                Start();
            
            // hide/show UI based on number of characters currently in the party
            UpdateUIShown();
        }

        void SetAllUI(PlayerCharacter activeCharacter)
        {
            // ensure UI icons length is within range of character party
            if (UIIcons.Length < characterManager.character_instances.Length)
            {
                Debug.LogError("There are more character instances than UI icons provided. (PlayerCharacterUIProfileManager.cs)");
                return;
            }

            // Counter for indexing UIIcons. Start at 1 as first UIIcon is reserved for active character.
            int c = 1;
            // loop through characters in the party and set the active character
            for (int i = 0; i < characterManager.character_instances.Length; i++)
            {
                // get the current character
                PlayerCharacter character = characterManager.character_instances[i];

                // If active character, use first ui profile slot.
                if (character == activeCharacter)
                {
                    // Assign active character to the reserved UIIcon
                    UIIcons[0].SetUI(activeCharacter);
                    continue;
                }
                UIIcons[c].SetUI(character);
                c++; // Increment counter to get next UIIcon
            }
        }

        void UpdateUIShown()
        {
            // ensure character manager is not null
            if (characterManager == null) return;
            // check if there are more UI elements than characters in the party
            if (UIIcons.Length <= characterManager.character_instances.Length) return;
            // hide UI that does not have a correspoinding character in the party
            for (int i = 0; i < (UIIcons.Length - characterManager.character_instances.Length); i++)
            {
                // do not hide first icon (active icon)
                if (i == (UIIcons.Length - 1)) break;
                // hide UI element
                UIIcons[UIIcons.Length - (i + 1)].gameObject.SetActive(false);
            }
        }

        void OnCharacterChange(PlayerCharacter prevCharacter, PlayerCharacter newCharacter)
        {
            // do not switch to character if character cannot be switched into
            if (!newCharacter.Switchable) return;
            // update UI icons
            SetAllUI(newCharacter);
        }
    }
}
