using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Level.Bins;
using NPC;
using Level;

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

        #region Spawned Recyclable Handlers
        private FSMRecyclableNPC[] recyclables;
        public FSMRecyclableNPC[] Recyclables
        {
            get
            {
                GetNPCCount();
                return recyclables;
            }
        }

        private Dictionary<RecyclableType, int> npcCount = new Dictionary<RecyclableType, int>();
        public Dictionary<RecyclableType, int> NPCCount
        {
            get
            {
                GetNPCCount();
                return npcCount;
            }
        }

        void GetNPCCount()
        {
            npcCount.Clear();
            if (LevelManager.Instance == null) return;
            recyclables = LevelManager.Instance.current_zone.GetComponentsInChildren<FSMRecyclableNPC>();
            if (recyclables == null || recyclables.Length <= 0) return;

            // count number of each recyclable
            foreach (FSMRecyclableNPC recyclable in recyclables)
            {
                RecyclableType type = recyclable.recyclableType;

                if (!npcCount.ContainsKey(type))
                    npcCount.Add(type, 1);
                else
                    npcCount[type]++;
            }
        }
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
