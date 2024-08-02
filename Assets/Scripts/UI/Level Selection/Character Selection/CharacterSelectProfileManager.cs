using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterSelectProfileManager : MonoBehaviour
    {
        [SerializeField] RectTransform parent;
        [SerializeField] GameObject prefab;
        PlayerCharacterSO[] characters => GameManager.Instance.config.characters;
        List<GameObject> objectPool = new List<GameObject>();

        public void LoadCharacters()
        {
            for (int i = 0; i < characters.Length; i++)
            {
                PlayerCharacterSO character = characters[i];
                GameObject obj = GetObject(i);
                obj.SetActive(true);
                obj.GetComponent<CharacterSelectProfile>()?.SetCharacter(character);
            }

            if (objectPool.Count <= characters.Length) return;

            for (int i = characters.Length; i < objectPool.Count - characters.Length; i++)
            {
                objectPool[i].SetActive(false);
            }
        }

        GameObject GetObject(int index)
        {
            if (index < objectPool.Count) return objectPool[index];
            GameObject obj = Instantiate(prefab, parent);
            objectPool.Add(obj);
            return obj;
        }
    }
}
