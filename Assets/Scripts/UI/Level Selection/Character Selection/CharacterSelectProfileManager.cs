using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterSelectProfileManager : MonoBehaviour
    {
        [SerializeField] RectTransform parent;
        [SerializeField] GameObject prefab;
        PlayerCharacterSO[] characters => GameManager.Instance.config.characters;
        public List<CharacterSelectProfile> objectPool { get; private set; } = new List<CharacterSelectProfile>();

        public event Action<CharacterSelectProfile> CharacterProfileCreated;

        public void LoadCharacters()
        {
            for (int i = 0; i < characters.Length; i++)
            {
                PlayerCharacterSO character = characters[i];
                CharacterSelectProfile obj = GetObject(i);
                obj.gameObject.SetActive(true);
                obj.HideBorder();
                obj.SetCharacter(character);
            }

            if (objectPool.Count <= characters.Length) return;

            for (int i = characters.Length; i < objectPool.Count - characters.Length; i++)
            {
                objectPool[i].gameObject.SetActive(false);
            }
        }

        CharacterSelectProfile GetObject(int index)
        {
            if (index < objectPool.Count) return objectPool[index];
            GameObject obj = Instantiate(prefab, parent);
            objectPool.Add(obj.GetComponent<CharacterSelectProfile>());
            CharacterProfileCreated?.Invoke(objectPool[^1]);
            return objectPool[^1];
        }
    }
}
