using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Interfaces;
using Bosses.Pilotras.FSM;
using Level.Bins;
using Random = UnityEngine.Random;

namespace Bosses.Pilotras.Bin
{
    public class PilotrasBinManager : MonoBehaviour
    {
        // references
        PilotrasController character;
        BinDropState binDrop => character.BinDropState;

        // variables for calculations
        [HideInInspector] public List<RecyclingBin> selectedBins = new List<RecyclingBin>();
        [HideInInspector] public float scoredInInstance;

        void Start()
        {
            // get character controller component
            character = GetComponentInParent<PilotrasController>();
        }

        void LateUpdate()
        {
            // ensure all selected bins are hidden when outside of bin drop state
            if (character.currentState == binDrop) return;
            if (selectedBins.Count <= 0) return;

            foreach (RecyclingBin bin in selectedBins)
            {
                if (!bin.gameObject.activeInHierarchy) continue;
                bin.gameObject.SetActive(false);
            }

            selectedBins.Clear();
        }

        public void ResetBin(RecyclingBin bin)
        {
            // unsubcribe from bin scored event
            bin.BinScored -= BinScored;

            // ensure bin score dictionary contains recyclable type
            if (!character.data.binScore.ContainsKey(bin.recyclableType))
            {
                Debug.LogWarning("Cannot find recyclable type in score dictionary. (BinDropState.cs)");
                return;
            }

            // store bin score depending on if the bin got contaminated
            if (bin.binState != BinState.CLEAN)
                character.data.binScore[bin.recyclableType] += bin.Score;
            else 
                character.data.binScore[bin.recyclableType] = bin.Score;
            
            // reset bin score
            bin.CompleteClean();
            bin.Score = character.data.binScore[bin.recyclableType];
        }

        public void DropBin()
        {
            // play animation
            character.anim?.Play("Slam");
            // stun all enemies within zone
            binDrop.StunNPCs();
            
            // push back NPCs under drop location
            binDrop.SendShockwave(binDrop.shockwaveCenter, binDrop.shockwaveSize);
            // halve shockwave size for debugging method
            binDrop.shockwaveSize *= 0.5f;

            // drop bin to designated location
            for (int i = 0; i < selectedBins.Count; i++)
            {
                GameObject bin = selectedBins[i].gameObject;
                binDrop.xPos += i * character.behaviourData.bin_spacing;
                bin.transform.position = new Vector2(binDrop.xPos, character.yPosTop);
                binDrop.coroutineManager.StartCoroutine(character.Throw(character.behaviourData.bin_drop_speed, 
                    character.behaviourData.bin_drop_delay, bin, new Vector2(binDrop.xPos, binDrop.yPos)));
                binDrop.coroutineManager.StartCoroutine(binDrop.coroutineManager.DelayedBinEnable(selectedBins[i], i == 0));
            }
        }

        public void RaiseBin()
        {
            character.anim?.Play("Slam (Reverse)");

            // reset bin points in this drop instance
            scoredInInstance = 0f;
            // return each bin
            foreach (RecyclingBin bin in selectedBins)
            {
                // disable bin script when lifting up bin
                bin.enabled = false;
                // start coroutines to lift bin
                binDrop.coroutineManager.StartCoroutine(character.Throw(character.behaviourData.bin_drop_speed, bin.gameObject, 
                    new Vector2(bin.transform.position.x, character.yPosTop)));
                binDrop.coroutineManager.StartCoroutine(binDrop.coroutineManager.DelayedBinInactive(bin));
                // reset bin
                ResetBin(bin);
            }
        }

        public void LoadBins()
        {
            if (character.levelManager == null || character.data.spawnedBins == null || character.data.spawnedBins.Length <= 0)
            {
                Debug.LogWarning("Bins could not be loaded! (BinDropState.cs)");
                return;
            }
            
            // array to store bins that can be used
            RecyclingBin[] binsFound = new RecyclingBin[0];
            // store bin to be selected
            RecyclingBin usableBin = null;
            // store the recycling type with the largest count, others is the default because it would not count contaminants
            RecyclableType maxType = RecyclableType.OTHERS;

            // check if dictionary contains more than 1 key (need at least 2 items to aggregate)
            if (character.data.npcCount.Count > 1)
            {
                // search for recycling type with the highest number of NPCs
                RecyclableType[] selectedTypes = selectedBins.Select(x => x.recyclableType).ToArray();
                maxType = character.data.npcCount
                    .Where(x => !selectedTypes.Contains(x.Key) && x.Key != RecyclableType.OTHERS)
                    .Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            }
            else if (character.data.npcCount.Count == 1)
            {
                maxType = character.data.npcCount.Keys.ToArray()[0];
            }

            // log type calculation proccess
            if (maxType == RecyclableType.OTHERS)
                Debug.Log("Unable to find bins with the most number of NPCs on the field, picking a random bin...");
            else
                Debug.Log($"maxType: {maxType}, count: {character.data.npcCount[maxType]}");

            // search for active bins that have the same type
            binsFound = character.data.spawnedBins
                .Where(x => x.recyclableType == maxType)
                .ToArray();
            
            // if no bins are found, try to find a random bin that is not selected
            if (binsFound.Length <= 0)
            {
                binsFound = character.data.spawnedBins
                    .Where(x => !selectedBins
                        .Select(x => x.recyclableType)
                        .ToArray()
                        .Contains(x.recyclableType))
                    .ToArray();
            }
            else
            {
                usableBin = binsFound[0];
            }

            // if there are still no bins found, log a warning
            if (binsFound == null || binsFound.Length <= 0)
            {
                Debug.LogWarning($"Unable to find a usable bin to drop. (BinDropState.cs)");
                return;
            }
            // if there are bin found, but no usable bin is selected, select a random bin
            else if (usableBin == null)
            {
                Debug.Log("Successfully selected a random bin.");
                usableBin = binsFound[Random.Range(0, binsFound.Length)];
            }
            
            // subscribe to bin scored event and add to selected bin list
            usableBin.BinScored += BinScored;
            // disable bin script while still dropping down
            usableBin.enabled = false;
            // add bin to selected bin list
            selectedBins.Add(usableBin);
        }

        void BinScored(float scoreChange, RecyclableType? type)
        {
            if (scoreChange > 0)
            {
                character.GetComponent<IDamagable>()?
                    .Damage(character.behaviourData.scored_damage * scoreChange);
                scoredInInstance += scoreChange;
                return;
            }

            // play roaring sound effect
            character.Roar();
            // heal self
            character.GetComponent<IDamagable>()?
                .Damage(-character.behaviourData.contaminated_heal * 
                    (scoreChange >= 0f ? 1f : Mathf.Abs(scoreChange)));
            // immidiately exit state
            character.SwitchState(character.PostBinDropStunState);
            
            if (type == null || type != RecyclableType.ELECTRONICS) return;
            character.fireController?.SpawnFire();
        }
    }
}
