using System;
using System.Collections.Generic;
using UnityEngine;
using Level.Bins;
using NPC;

namespace Bosses.Pilotras
{
    [Serializable]
    public struct PhaseObjects
    {
        public GameObject[] gameObjects;
    }

    public class PilotrasData : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] EntitySO data;
        [SerializeField] float maxHealth = 500f;
        [SerializeField] int numberOfPhases = 2;
        [SerializeField] PhaseObjects[] spawnableNPC;
        [SerializeField] PhaseObjects[] spawnableBins;
        #endregion

        #region Public Properties
        public EntitySO entityData => data;
        public float max_health => maxHealth;
        public int number_of_phases => numberOfPhases;
        public PhaseObjects[] spawnable_npcs => spawnableNPC;
        public PhaseObjects[] spawnable_bins => spawnableBins;
        #endregion

        #region Hidden Public Variables
        [HideInInspector] public GameObject[] currentPhaseNPCs;
        [HideInInspector] public RecyclingBin[] spawnedBins;
        [HideInInspector] public FSMRecyclableNPC[] recyclables;
        [HideInInspector] public Dictionary<RecyclableType, int> npcCount = new Dictionary<RecyclableType, int>();
        [HideInInspector] public Dictionary<RecyclableType, float> binScore = new Dictionary<RecyclableType, float>();
        [HideInInspector] public float damageTakenScale = 1f;
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            if (spawnableNPC == null)
                Debug.LogWarning("Spawnable NPCs array is null, there are no NPCs to spawn! (PilotrasData.cs)");
            else if (spawnableNPC.Length != numberOfPhases)
                Debug.LogWarning("Spawnable NPCs array should be the length of the number of phases. (PilotrasData.cs)");
        }
        #endregion
    }
}
