using UnityEngine;

public class Character : MonoBehaviour {
    [Header("Character Information")]
    public string character_name;
    [TextArea(3, 10)]
    public string character_desc;

    public void Spawn(){
        gameObject.SetActive(true);
    }
    public void Despawn(){
        gameObject.SetActive(false);
    }
}