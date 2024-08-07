using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Patterns.FSM;
using Interfaces;
using Level.Bins;
using Random = UnityEngine.Random;

namespace Bosses.Pilotras.FSM
{
    public class BinDropState : CooldownState<PilotrasController>
    {
        List<RecyclingBin> selectedBins = new List<RecyclingBin>();
        float yPosTop => character.levelManager.current_zone.center.y + (character.levelManager.current_zone.size.y / 2f);

        public BinDropState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.bin_drop_duration, character.behaviourData.bin_drop_cooldown)
        {
        }

        public override void Enter()
        {
            duration = character.behaviourData.bin_drop_duration;
            cooldown = character.behaviourData.bin_drop_cooldown;
            base.Enter();

            // update current number of NPCs and reset selected bins
            character.UpdateNPCCount();
            selectedBins.Clear();

            // load bins to be dropped
            LoadBins();
            // load second bin if phase is more than one
            if (character.currentPhase > 1) LoadBins();

            // stun all enemies within zone
            StunNPCs();

            // set bin location and activate bin
            Vector2 binContainerLocation = (Vector2) character.behaviourData.active_bins.position + character.behaviourData.bin_offset;
            float xPos = binContainerLocation.x - (character.behaviourData.bin_spacing * ((selectedBins.Count - 1f) / 2f));
            float yPos = binContainerLocation.y;

            // push back NPCs under drop location
            PushBackNPCs(binContainerLocation, new Vector2(xPos * 2f, yPos * 2f));

            // drop bin to designated location
            for (int i = 0; i < selectedBins.Count; i++)
            {
                GameObject bin = selectedBins[i].gameObject;
                bin.transform.parent = character.behaviourData.active_bins;
                xPos += i * character.behaviourData.bin_spacing;
                bin.transform.position = new Vector2(xPos, yPosTop);
                character.StartCoroutine(character.Throw(character.behaviourData.bin_drop_speed, bin, new Vector2(xPos, yPos)));
            }
        }

        public override void Exit()
        {
            base.Exit();
            // return each bin to inactive bins
            foreach (RecyclingBin bin in selectedBins)
            {
                bin.transform.parent = character.behaviourData.inactive_bins;
            }
        }

        void PushBackNPCs(Vector2 center, Vector2 detectionSize)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(center, detectionSize, 
                character.behaviourData.drop_detection_mask);
            Rigidbody2D hitRB;

            // apply knockback to hit NPCs
            foreach (Collider2D hit in hits)
            {
                if (hit.transform == character.transform) continue;
                hitRB = hit.GetComponent<Rigidbody2D>();
                if (hitRB == null) continue;
                hitRB.AddForce((hit.transform.position - character.transform.position).normalized * 
                    character.behaviourData.bin_drop_force, ForceMode2D.Impulse);
            }
        }

        void StunNPCs()
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(character.levelManager.current_zone.transform.position, 
                (Vector2) character.levelManager.current_zone.center + (character.levelManager.current_zone.size / 2f), 
                character.behaviourData.drop_detection_mask);

            foreach (Collider2D hit in hits)
            {
                if (hit.transform == character.transform) continue;
                hit.GetComponent<IStunnable>()?.Stun(duration);
            }
        }

        void LoadBins()
        {
            if (character.levelManager == null || character.data.spawnedBins == null || character.data.spawnedBins.Length <= 0)
                return;
            
            // search for recycling type with the highest number of NPCs
            RecyclableType[] selectedTypes = selectedBins.Select(x => x.recyclableType).ToArray();
            RecyclableType maxType = character.data.npcCount
                .Aggregate((l, r) => l.Value > r.Value && Array.IndexOf(selectedTypes, l.Key) == -1 ? l : r).Key;
            
            Debug.Log($"maxType: {maxType}, count: {character.data.npcCount[maxType]}");

            // search for active bins that have the same type
            RecyclingBin[] binsFound = character.data.spawnedBins
                .Where(x => x.recyclableType == maxType)
                .ToArray();

            // store bin to be selected
            RecyclingBin usableBin = null;
            
            // if no bins are found, try to find a random bin that is not selected
            if (binsFound == null || binsFound.Length <= 0)
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
                Debug.LogWarning($"Unable to find bin of type {maxType} (BinDropState.cs)");
                return;
            }
            else
            {
                usableBin = binsFound[Random.Range(0, binsFound.Length)];
            }
            
            selectedBins.Add(usableBin);
        }
    }
}
