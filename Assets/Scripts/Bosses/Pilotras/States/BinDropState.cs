using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Patterns.FSM;
using Level.Bins;

namespace Bosses.Pilotras.FSM
{
    public class BinDropState : CoroutineState<PilotrasController>
    {
        List<RecyclingBin> selectedBins = new List<RecyclingBin>();

        public BinDropState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.placing_duration)
        {
        }

        public override void Enter()
        {
            base.Enter();
            selectedBins.Clear();
        }

        RecyclingBin GetBin()
        {
            if (character.levelManager == null || character.data.spawnable_bins == null || character.data.spawnable_bins.Length <= 0 ||
                character.data.spawnable_bins[0].gameObjects == null || character.data.spawnable_bins[0].gameObjects.Length <= 0)
                    return null;
            
            RecyclableType[] selectedTypes = selectedBins.Select(x => x.recyclableType).ToArray();
            RecyclableType maxType = character.data.NPCCount
                .Aggregate((l, r) => l.Value > r.Value && Array.IndexOf(selectedTypes, l.Key) == -1 ? l : r).Key;

            GameObject[] placableBins = character.data.spawnable_bins[0].gameObjects;
            
            for (int i = 1; i < character.currentPhase; i++)
            {
                if (character.data.spawnable_bins.Length <= i) break;

                placableBins = placableBins
                    .Concat(character.data.spawnable_bins[i].gameObjects)
                    .Where(x => x != null)
                    .ToArray();
            }

            if (placableBins == null || placableBins.Length <= 0) return null;

            RecyclingBin[] bins = placableBins
                .Select(x => x.GetComponent<RecyclingBin>())
                .Where(x => x != null)
                .ToArray();
            
            if (bins == null || bins.Length <= 0) return null;

            return bins[0];
        }
    }
}
