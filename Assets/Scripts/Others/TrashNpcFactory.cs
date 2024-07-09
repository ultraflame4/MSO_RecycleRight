using UnityEngine;

[CreateAssetMenu(fileName = "TrashNpcFactory", menuName = "ScriptableObjects/TrashNpcFactory", order = 0)]
public class TrashNpcFactory : ScriptableObject {
    public GameObject templateRecyclablePrefab;
    public GameObject templateContaminantPrefab;
}