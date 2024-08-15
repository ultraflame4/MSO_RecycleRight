using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Level.Bins;
using NPC;

namespace Bosses.Pilotras
{
    public class PilotrasNPCSpawner : MonoBehaviour
    {
        // get reference to controller script
        [HideInInspector] public PilotrasController character;

        // private variables for calculation
        Dictionary<RecyclableType, List<GameObject>> prefabs;
        Dictionary<RecyclableType, float> weight;

        // default output
        GameObject defaultOutput => character.data.currentPhaseNPCs[Random.Range(0, character.data.currentPhaseNPCs.Length)];

        /// <summary>
        /// Load NPCs into a dictionary sorted by recycling type for calculations
        /// </summary>
        public void LoadNPCs()
        {
            // ensure there are spawnable NPCs
            if (character.data.currentPhaseNPCs == null) return;

            // reset prefabs dictionary
            if (prefabs == null)
                prefabs = new Dictionary<RecyclableType, List<GameObject>>();
            else
                prefabs.Clear();
            
            // loop through all spawnable NPCs and sort them by recycling type
            foreach (GameObject obj in character.data.currentPhaseNPCs)
            {
                FSMRecyclableNPC npc = obj.GetComponent<FSMRecyclableNPC>();
                if (npc == null) continue;

                if (!prefabs.ContainsKey(npc.recyclableType))
                    prefabs.Add(npc.recyclableType, new List<GameObject>());
                
                // do not add duplicate
                if (prefabs[npc.recyclableType].Contains(obj)) continue;
                // add object to dictionary
                prefabs[npc.recyclableType].Add(obj);
            }
        }

        public GameObject GetNPC()
        {
            // update NPC count before calculating which NPC to return
            // actually, this should be called in the method calling this to avoid being called too much as it is inefficient
            // character.UpdateNPCCount();

            if (prefabs.Keys.Count < 2)
                return defaultOutput;

            // reset weight
            if (weight == null)
                weight = new Dictionary<RecyclableType, float>();
            else
                weight.Clear();
            
            // get total weight
            float npcSum = character.data.npcCount.Values.Sum(x => x);
            
            // calculate chance for each recycling type
            for (int i = 0; i < prefabs.Keys.Count; i++)
            {
                RecyclableType type = prefabs.Keys.ToArray()[i];

                if (!character.data.npcCount.ContainsKey(type))
                    weight.Add(type, 1);
                else
                    weight.Add(type, Mathf.Clamp01(1f - (character.data.npcCount[type] / npcSum)));
            }

            float randomValue = Random.Range(0f, 1f);
            RecyclableType? selectedType = null;

            // search for a type that has a higher weight than the random value
            foreach (RecyclableType type in weight.Keys)
            {
                Debug.Log($"{type}, {weight[type]} - selected: {selectedType}");
                // pass if weight is below random value, or currently selected value has a higher weight
                if (randomValue > weight[type] || (selectedType != null && 
                    weight[type] < weight[(RecyclableType) selectedType])) 
                        continue;
                // if two objects have the same weight, randomely select 1 of them
                if (selectedType != null && weight[type] == weight[(RecyclableType) selectedType])
                {
                    int random = Random.Range(0, 2);
                    if (random == 0) continue;
                }
                // assign selected type
                selectedType = type;
            }

            // if none of the types got selected, return default output (pure random chance)
            if (selectedType == null) return defaultOutput;
            return prefabs[(RecyclableType) selectedType][Random.Range(0, prefabs[(RecyclableType) selectedType].Count)];
        }
    }
}
