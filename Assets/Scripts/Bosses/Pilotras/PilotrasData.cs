using System;
using UnityEngine;

namespace Bosses.Pilotras
{
    [Serializable]
    public struct SpawnableInPhase
    {
        public GameObject[] npcs;
    }

    public class PilotrasData : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] EntitySO data;
        [SerializeField] float maxHealth = 500f;
        [SerializeField] int numberOfPhases = 2;
        [SerializeField] SpawnableInPhase[] spawnableNPC;
        #endregion

        #region Public Properties
        public EntitySO entityData => data;
        public float max_health => maxHealth;
        public int number_of_phases => numberOfPhases;
        public SpawnableInPhase[] spawnable_npcs => spawnableNPC;
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
