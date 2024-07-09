using System.IO;
using NPC.Contaminant;
using NPC.Recyclable;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashNpcFactory", menuName = "ScriptableObjects/TrashNpcFactory", order = 0)]
public class TrashNpcFactorySO : ScriptableObject
{
    public GameObject templateRecyclablePrefab;
    public GameObject templateContaminantPrefab;

    

    public void CreateRecyclable(string filepath, TrashNpcSO data)
    {
        // modified from, https://forum.unity.com/threads/solved-creating-prefab-variant-with-script.546358/#post-3605645
        GameObject prefab = PrefabUtility.InstantiatePrefab(templateRecyclablePrefab) as GameObject;
        prefab.name = Path.GetFileNameWithoutExtension(filepath);
        prefab.GetComponent<RecyclableNPC>().npcData = data;
        PrefabUtility.SaveAsPrefabAsset(prefab, filepath);
    }
    public void CreateContaminant(string filepath, TrashNpcSO data)
    {
        // modified from, https://forum.unity.com/threads/solved-creating-prefab-variant-with-script.546358/#post-3605645
        GameObject prefab = PrefabUtility.InstantiatePrefab(templateContaminantPrefab) as GameObject;
        prefab.name = Path.GetFileNameWithoutExtension(filepath);
        prefab.GetComponent<ContaminantNPC>().npcData = data;
        PrefabUtility.SaveAsPrefabAsset(prefab, filepath);
    }
}