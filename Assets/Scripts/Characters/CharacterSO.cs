using UnityEngine;


[CreateAssetMenu(fileName = "Character_", menuName = "Character", order = 0)]
public class CharacterSO : ScriptableObject {
    public string name = "New Character";
    public string description =" Lorem Ipsum";
    public GameObject prefab;


    public Character character => prefab.GetComponent<Character>();

    private void OnValidate() {
        if (prefab == null){
            Debug.LogError("Prefab should not be null!");
            return;
        }

        if (!prefab.TryGetComponent<Character>(out _)){
            Debug.LogError("Prefab must have a Character component attached to it!");
        }
    }
}