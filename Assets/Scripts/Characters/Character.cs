using UnityEngine;

public class Character : MonoBehaviour {
    [Header("Character Information")]
    public string characterName;
    [TextArea(3, 10)]
    public string characterDesc;

    public void SetSpawn(bool active)
    {
        gameObject.SetActive(active);
    }

    // public void Spawn(){
    //     gameObject.SetActive(true);
    // }
    // public void Despawn(){
    //     gameObject.SetActive(false);
    // }
}