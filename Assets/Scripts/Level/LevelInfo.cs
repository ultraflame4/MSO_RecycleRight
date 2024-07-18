using UnityEngine;

/// <summary>
/// Information about the npcs in the level.
/// </summary>
public struct LevelNPC{
    /// <summary>
    /// The data about the level npc
    /// </summary>
    TrashNpcSO npcData;
    /// <summary>
    /// Whether the npc spawns as a contaminant in the level
    /// </summary>
    bool is_contaminant;
    /// <summary>
    /// Number of npcs of this type.
    /// </summary>
    int count;
}

/// <summary>
/// Scriptable object to hold information about the level.
/// </summary>
[CreateAssetMenu(fileName = "LevelInfo", menuName = "LevelInfo", order = 0)]
public class LevelInfo : ScriptableObject {
    public string levelName;
    public string levelDescription;
    public float maxScore;
    public LevelNPC[] npcs;
}