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
        Vector2 binContainerLocation, shockwaveCenter, shockwaveSize;
        List<Coroutine> coroutine_throw, coroutine_enable;
        Coroutine coroutine_drop;
        GameObject indicator;
        float xPos, yPos, scoredInInstance;

        public BinDropState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.PostBinDropStunState, 
                character.behaviourData.bin_drop_duration + character.data.attack_delay, 
                character.behaviourData.bin_drop_cooldown)
        {
            coroutine_throw = new List<Coroutine>();
            coroutine_enable = new List<Coroutine>();
        }

        public override void Enter()
        {
            duration = character.behaviourData.bin_drop_duration + 
                character.data.attack_delay;
            cooldown = character.behaviourData.bin_drop_cooldown;
            base.Enter();

            // update current number of NPCs and reset selected bins
            character.UpdateNPCCount();
            selectedBins.Clear();
            // reset bin points in this drop instance
            scoredInInstance = 0f;
            // reset coroutine lists
            coroutine_throw = ResetCoroutines(coroutine_throw);
            coroutine_enable = ResetCoroutines(coroutine_enable);
            coroutine_throw.Clear();
            coroutine_enable.Clear();

            // load bins to be dropped
            LoadBins();
            // load second bin if phase is more than one
            if (character.currentPhase > 1) LoadBins();

            // start coroutine to count delay before dropping bin
            coroutine_drop = character.StartCoroutine(DelayedBinDrop());
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
            character.anim?.Play("Slam (Reverse)");

            // stop bin drop coroutine
            if (coroutine_drop != null) character.StopCoroutine(coroutine_drop);
            coroutine_throw = ResetCoroutines(coroutine_throw);
            coroutine_enable = ResetCoroutines(coroutine_enable);
            // reset indicator
            indicator?.SetActive(false);
            indicator = null;

            // reset bin points in this drop instance
            scoredInInstance = 0f;
            // return each bin
            foreach (RecyclingBin bin in selectedBins)
            {
                // unsubcribe from bin scored event
                bin.BinScored -= BinScored;
                // disable bin script when lifting up bin
                bin.enabled = false;
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
                if (!character.data.binScore.ContainsKey(bin.recyclableType)) return;

                // store bin score depending on if the bin got contaminated
                if (bin.binState != BinState.CLEAN)
                    character.data.binScore[bin.recyclableType] += bin.Score;
                else 
                    character.data.binScore[bin.recyclableType] = bin.Score;
                
                // reset bin score
                bin.CompleteClean();
                bin.Score = character.data.binScore[bin.recyclableType];
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

        List<Coroutine> ResetCoroutines(List<Coroutine> coroutines)
        {
            foreach (Coroutine coroutine in coroutines)
            {
                if (coroutine == null) continue;
                character.StopCoroutine(coroutine);
            }

            return coroutines;
        }

        void DropBin()
        {
            // play animation
            character.anim?.Play("Slam");
            // stun all enemies within zone
            StunNPCs();

            // push back NPCs under drop location
            SendShockwave(shockwaveCenter, shockwaveSize);
            // halve shockwave size for debugging method
            shockwaveSize *= 0.5f;

            // drop bin to designated location
            for (int i = 0; i < selectedBins.Count; i++)
            {
                GameObject bin = selectedBins[i].gameObject;
                xPos += i * character.behaviourData.bin_spacing;
                bin.transform.position = new Vector2(xPos, character.yPosTop);
                coroutine_throw.Add(character.StartCoroutine(character.Throw(character.behaviourData.bin_drop_speed, 
                    character.behaviourData.bin_drop_delay, bin, new Vector2(xPos, yPos))));
                coroutine_enable.Add(character.StartCoroutine(DelayedBinEnable(selectedBins[i])));
            }
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

            hits = Physics2D.OverlapBoxAll(center, detectionSize, 
                character.data.hit_mask);
            
            // apply knockback and damage to player
            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent<IDamagable>(out IDamagable damagable))
                    damagable.Damage(character.data.bin_drop_damage);

                if (hit.TryGetComponent<IStunnable>(out IStunnable stunnable))
                    stunnable.Stun(character.data.bin_drop_player_stun_duration);

                hitRB = hit.GetComponentInParent<Rigidbody2D>();
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
                if (!recyclable.TryGetComponent<IStunnable>(out IStunnable stunnable)) continue;
                stunnable.Stun(duration);
            }
        }

        void LoadBins()
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
                .Damage(character.behaviourData.contaminated_heal * scoreChange);
            // immidiately exit state
            fsm.SwitchState(nextState);
            
            if (type == null || type != RecyclableType.ELECTRONICS) return;
            character.fireController?.SpawnFire();
        }

        IEnumerator DelayedBinDrop()
        {
            // set bin location and activate bin
            binContainerLocation = (Vector2) character.transform.position + character.behaviourData.bin_offset;
            xPos = binContainerLocation.x - (character.behaviourData.bin_spacing * ((selectedBins.Count - 1f) / 2f));
            yPos = binContainerLocation.y;

            // set shockwave values
            shockwaveSize = new Vector2(Mathf.Abs(xPos) + character.behaviourData.bin_spacing, 
                character.yPosTop - binContainerLocation.y + character.behaviourData.bin_spacing);
            shockwaveCenter = binContainerLocation;
            shockwaveCenter.y += (shockwaveSize.y - character.behaviourData.bin_spacing) / 2f;

            // show indicator
            Vector2 indicatorScale = shockwaveSize;
            indicatorScale.y *= 0.5f;
            indicator = character.indicatorManager.Instantiate(2, 
                new Vector2(shockwaveCenter.x, shockwaveCenter.y - (indicatorScale.y * 0.5f)));
            Transform indicatorSprite = indicator.transform.GetChild(0);
            indicatorSprite.localScale *= indicatorScale;

            // delay the attack by the attack indicator
            yield return new WaitForSeconds(character.data.attack_delay);

            // reset coroutine to null
            coroutine_drop = null;
            // hide indicator after attack delay duration
            indicatorSprite.localScale /= indicatorScale;
            indicator.SetActive(false);
            // drop bin after waiting for attack delay
            DropBin();
        }

        IEnumerator DelayedBinEnable(RecyclingBin bin)
        {
            yield return new WaitForSeconds(character.behaviourData.bin_drop_delay + 
                character.behaviourData.bin_drop_speed);
            bin.enabled = true;
            // play sfx when bin landed on ground
            SoundManager.Instance?.PlayOneShot(character.data.sfx_bin_drop);
            // shake camera for effects
            character.levelManager?.camera?.ShakeCamera(0.5f);
        }

        IEnumerator DelayedBinInactive(RecyclingBin bin)
        {
            yield return new WaitForSeconds(character.behaviourData.bin_drop_speed);
            bin?.gameObject.SetActive(false);
        }
    }
}
