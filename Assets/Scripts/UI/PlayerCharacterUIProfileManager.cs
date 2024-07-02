using UnityEngine;
using Entity.Data;
using Player;

namespace UI
{
    public class PlayerCharacterUIProfileManager : MonoBehaviour
    {
        [SerializeField] PlayerController player;
        [SerializeField] PlayerCharacterUIProfile[] UIIcons;

        // Start is called before the first frame update
        void Start()
        {
            // ensure player is not null
            if (player == null) return;
            // subscribe to character change event
            player.CharacterManager.CharacterChanged += OnCharacterChange;
            // start by setting all UI icons
            SetAllUI(player.CharacterManager.character_instances[0]);
        }

        // Update is called once per frame
        void Update()
        {
            // hide/show UI based on number of characters currently in the party
            UpdateUIShown();
        }

        void SetAllUI(PlayerCharacter activeCharacter)
        {
            // ensure UI icons length is within range of character party
            if (UIIcons.Length < player.CharacterManager.character_instances.Length)
            {
                Debug.LogError("There are more character instances than UI icons provided. (PlayerCharacterUIProfileManager.cs)");
                return;
            }

            // cache first character in array
            PlayerCharacter cachedCharacter =  player.CharacterManager.character_instances[0];
            // loop through characters in the party and set the active character
            for (int i = 0; i < player.CharacterManager.character_instances.Length; i++)
            {
                // get the current character
                PlayerCharacter character = player.CharacterManager.character_instances[i];
                // variable to change depending on the character to set
                PlayerCharacter characterToSet = character;

                // if first character is not the active, directly set first UI to active character
                if (cachedCharacter != activeCharacter && i == 0)
                {
                    // set first character in active UI spot
                    characterToSet = activeCharacter;
                    // cache current character
                    cachedCharacter = character;
                }

                // if reached the current location of the active character, set the UI to the previously cached character
                if (character == activeCharacter && i != 0) characterToSet = cachedCharacter;

                UIIcons[i].SetUI(player, characterToSet);
            }
        }

        void UpdateUIShown()
        {
            // ensure player is not null
            if (player == null) return;
            // check if there are more UI elements than characters in the party
            if (UIIcons.Length <= player.CharacterManager.character_instances.Length) return;
            // hide UI that does not have a correspoinding character in the party
            for (int i = 0; i < (UIIcons.Length - player.CharacterManager.character_instances.Length); i++)
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
