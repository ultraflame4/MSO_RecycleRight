using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [field: SerializeField, Tooltip("The container that will hold the characters.")]
    public Transform container { get; private set; }
    [field: SerializeField, Tooltip("The characters that the player can switch between.")]
    public GameObject[] characters { get; private set; }
    [field: SerializeField, Tooltip("A temporary placeholder for the character.")]
    public GameObject placeholder { get; private set; }
    public Character[] character_instances { get; private set; }

    public void Start()
    {
        placeholder.SetActive(false);
        character_instances = new Character[characters.Length];
        for (int i = 0; i < characters.Length; i++)
        {
            GameObject new_character = Instantiate(characters[i], container);
            character_instances[i] = new_character.GetComponent<Character>();
            new_character.SetActive(false);
        }

        SwitchCharacter(0);
    }

    /// <summary>
    /// Switches the character to the one at the specified index.
    /// </summary>
    /// <param name="index"></param>
    public void SwitchCharacter(int index)
    {
        if (index < 0 || index >= character_instances.Length)
        {
            Debug.LogWarning("Tried to switch to an invalid character index.");
            return;
        }
        for (int i = 0; i < character_instances.Length; i++)
        {
            Character character = character_instances[i];
            if (i == index)
            {
                character.Spawn();
            }
            else
            {
                character.Despawn();
            }
        }
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