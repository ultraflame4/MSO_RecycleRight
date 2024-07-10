using System;
using UnityEngine;
using Entity.Data;

namespace Player
{
    public class CharacterManager : MonoBehaviour
    {
        [field: SerializeField, Tooltip("The container that will hold the characters.")]
        public Transform container { get; private set; }
        [field: SerializeField, Tooltip("The characters that the player can switch between.")]
        public GameObject[] characters { get; private set; }
        [field: SerializeField, Tooltip("A temporary placeholder for the character.")]
        public GameObject placeholder { get; private set; }
        public PlayerCharacter[] character_instances { get; private set; }

        [HideInInspector] public bool CanSwitchCharacters = true;

        /// <summary>
        /// Event that is triggered everytime the player changes character
        /// </summary>
        public event Action<PlayerCharacter, PlayerCharacter> CharacterChanged;

        public void Awake()
        {
            placeholder.SetActive(false);
            character_instances = new PlayerCharacter[characters.Length];
            // instantiate characters
            for (int i = 0; i < characters.Length; i++)
            {
                GameObject new_character = Instantiate(characters[i], container);
                character_instances[i] = new_character.GetComponent<PlayerCharacter>();
                new_character.GetComponent<Renderer>().enabled = false;
            }

            // ensure character instances is filled before setting first active character
            if (character_instances.Length <= 0) return;

            // set first character as active
            for (int i = 0; i < character_instances.Length; i++)
            {
                PlayerCharacter character = character_instances[i];
                character.SetSpawn(i == 0);
            }
        }

        /// <summary>
        /// Switches the character to the one at the specified index.
        /// </summary>
        /// <param name="index">Index of character to switch to</param>
        public void SwitchCharacter(int index)
        {
            // check if can switch characters
            if (!CanSwitchCharacters) return;
            // ensure index is within range
            if (index < 0 || index >= character_instances.Length)
            {
                Debug.LogWarning("Tried to switch to an invalid character index.");
                return;
            }
            // check if can switch to character
            if (!character_instances[index].Switchable) return;

            // cache previous character
            PlayerCharacter prev = null;
            // switch character
            for (int i = 0; i < character_instances.Length; i++)
            {
                PlayerCharacter character = character_instances[i];
                // do not disable characters that are cleaning
                if (character.IsCleaning) continue;
                // check if character is active
                if (character.Enabled) prev = character;
                // update spawn of character
                character.SetSpawn(i == index);
            }

            // invoke character switch event
            CharacterChanged?.Invoke(prev, character_instances[index]);
        }

        private void Update()
        {
            // todo: move all input handling to a separate input manager class.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SwitchCharacter(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwitchCharacter(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SwitchCharacter(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SwitchCharacter(3);
            }
        }
    }
}
