using System;
using System.Linq;
using NPC;
using NPC.Contaminant;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    [SerializeField]
    private LevelInfoSO _info;
    private LevelInfoSO info
    {
        get
        {
            if (_info == null)
            {
                throw new NullReferenceException("Level info is null");
            }
            return _info;
        }
    }

    public LevelInfoData Data => info.data;


    [SerializeField]
    private LevelInfoData data;

    /// <summary>
    /// Attempts to calculate the max score possible in this level.
    /// </summary>

    [EasyButtons.Button]
    private void CalculateMaxScore()
    {
        var components = GetComponentsInChildren<FSMRecyclableNPC>();
        Debug.Log($"Found {components.Length} FSMRecyclableNPC");
        var max = components
                    .Where(x => x.recyclableType != Level.Bins.RecyclableType.OTHERS || ((x as ContaminantNPC)?.cleanable ?? false))
                    .Select(x => 1)
                    .Sum();
        data.maxScore = max;
    }


    [EasyButtons.Button]
    private void LoadFromScriptableObject()
    {
        data = info.data;
    }


    [EasyButtons.Button]
    private void SaveToScriptableObject()
    {
        info.data = data;
    }
}