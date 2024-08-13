using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Patterns.FSM;
using Interfaces;
using Level.Bins;
using NPC;
using Random = UnityEngine.Random;

namespace Bosses.Pilotras.FSM
{
    public class BinDropState : CooldownState<PilotrasController>
    {
        List<RecyclingBin> selectedBins = new List<RecyclingBin>();
        Vector2 shockwaveCenter, shockwaveSize;
        float scoredInInstance;

        public BinDropState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.PostBinDropStunState, character.behaviourData.bin_drop_duration, character.behaviourData.bin_drop_cooldown)
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
            // reset bin points in this drop instance
            scoredInInstance = 0f;

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

            // set shockwave values
            shockwaveSize = new Vector2(Mathf.Abs(xPos) + character.behaviourData.bin_spacing, 
                character.yPosTop - binContainerLocation.y + character.behaviourData.bin_spacing);
            shockwaveCenter = binContainerLocation;
            shockwaveCenter.y += (shockwaveSize.y - character.behaviourData.bin_spacing) / 2f;
            // push back NPCs under drop location
            SendShockwave(shockwaveCenter, shockwaveSize);
            // halve shockwave size for debugging method
            shockwaveSize *= 0.5f;

            // drop bin to designated location
            for (int i = 0; i < selectedBins.Count; i++)
            {
                GameObject bin = selectedBins[i].gameObject;
                bin.transform.parent = character.behaviourData.active_bins;
                xPos += i * character.behaviourData.bin_spacing;
                bin.transform.position = new Vector2(xPos, character.yPosTop);
                character.StartCoroutine(character.Throw(character.behaviourData.bin_drop_speed, bin, new Vector2(xPos, yPos)));
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (scoredInInstance < character.behaviourData.topple_threshold) return;
            fsm.SwitchState(character.ToppleState);
        }

        public override void Exit()
        {
            base.Exit();
            // reset bin points in this drop instance
            scoredInInstance = 0f;
            // return each bin
            foreach (RecyclingBin bin in selectedBins)
            {
                // unsubcribe from bin scored event
                bin.BinScored -= BinScored;

                // start coroutines to lift bin
                character.StartCoroutine(character.Throw(character.behaviourData.bin_drop_speed, bin.gameObject, 
                    new Vector2(bin.transform.position.x, character.yPosTop)));
                character.StartCoroutine(DelayedBinInactive(bin));

                // ensure bin score dictionary contains recyclable type
                if (!character.data.binScore.ContainsKey(bin.recyclableType))
                {
                    Debug.LogWarning("Cannot find recyclable type in score dictionary. (BinDropState.cs)");
                    continue;
                }

                // store, update, and reset bin score
                character.data.binScore[bin.recyclableType] += bin.Score;
            }
        }

        /// <summary>
        /// Method to draw debug lines to show shockwave range
        /// </summary>
        public void DrawDebug()
        {
            if (shockwaveCenter == Vector2.zero || shockwaveSize == Vector2.zero) return;
            Vector2 topLeft = new Vector2(shockwaveCenter.x - shockwaveSize.x, shockwaveCenter.y + shockwaveSize.y);
            Vector2 topRight = new Vector2(shockwaveCenter.x + shockwaveSize.x, shockwaveCenter.y + shockwaveSize.y);
            Vector2 bottomLeft = new Vector2(shockwaveCenter.x - shockwaveSize.x, shockwaveCenter.y - shockwaveSize.y);
            Vector2 bottomRight = new Vector2(shockwaveCenter.x + shockwaveSize.x, shockwaveCenter.y - shockwaveSize.y);
            Debug.DrawLine(topLeft, topRight, Color.magenta);
            Debug.DrawLine(topRight, bottomRight, Color.magenta);
            Debug.DrawLine(bottomRight, bottomLeft, Color.magenta);
            Debug.DrawLine(bottomLeft, topLeft, Color.magenta);
        }

        void SendShockwave(Vector2 center, Vector2 detectionSize)
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
            foreach (FSMRecyclableNPC recyclable in character.data.recyclables)
            {
                if (recyclable == null) continue;
                recyclable.GetComponent<IStunnable>()?.Stun(duration);
            }
        }

        void LoadBins()
        {
            if (character.levelManager == null || character.data.spawnedBins == null || character.data.spawnedBins.Length <= 0)
                return;
            
            // search for recycling type with the highest number of NPCs
            RecyclableType[] selectedTypes = selectedBins.Select(x => x.recyclableType).ToArray();
            RecyclableType maxType = character.data.npcCount
                .Where(x => Array.IndexOf(selectedTypes, x.Key) == -1)
                .Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            
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
            
            // subscribe to bin scored event and add to selected bin list
            usableBin.BinScored += BinScored;
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

            character.GetComponent<IDamagable>()?
                .Damage(character.behaviourData.contaminated_heal * scoreChange);
            fsm.SwitchState(nextState);
            
            if (type == null || type != RecyclableType.ELECTRONICS) return;
            character.fireController?.SpawnFire();
        }

        IEnumerator DelayedBinInactive(RecyclingBin bin)
        {
            yield return new WaitForSeconds(character.behaviourData.bin_drop_speed);
            bin.transform.parent = character.behaviourData.inactive_bins;
            bin.CompleteClean();

            if (character.data.binScore.ContainsKey(bin.recyclableType)) 
                bin.Score = character.data.binScore[bin.recyclableType];
        }
    }
}
