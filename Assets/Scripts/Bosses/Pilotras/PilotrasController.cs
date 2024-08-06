using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bosses.Pilotras.FSM;
using Patterns.FSM;
using Level;

namespace Bosses.Pilotras
{
    public class PilotrasController : StateMachine<PilotrasController>
    {
        #region Data
        [field: SerializeField] public PilotrasData data { get; private set; }
        [field: SerializeField] public PilotrasBehaviourData behaviourData { get; private set; }
        #endregion

        #region States
        public DefaultState DefaultState { get; private set; }
        public PlacingState PlacingState { get; private set; }
        #endregion

        #region References
        LevelManager levelManager => LevelManager.Instance;
        #endregion

        #region Other Properties
        public int currentPhase { get; private set; } = 0;
        #endregion

        #region Public Methods
        /// <summary>
        /// Places a random NPC within the zone
        /// </summary>
        public void PlaceNPC()
        {
            if (LevelManager.Instance == null || data.spawnable_npcs == null || data.spawnable_npcs.Length <= 0 ||
                data.spawnable_npcs[0].npcs == null || data.spawnable_npcs[0].npcs.Length <= 0)
                    return;
            
            GameObject[] placableNPCs = data.spawnable_npcs[0].npcs;
            
            for (int i = 1; i < currentPhase; i++)
            {
                if (data.spawnable_npcs.Length <= i) break;
                placableNPCs.Concat(data.spawnable_npcs[i].npcs);
            }

            placableNPCs = placableNPCs.Where(x => x != null).ToArray();
            if (placableNPCs == null || placableNPCs.Length <= 0) return;

            LevelZone currentZone = LevelManager.Instance.current_zone;
            Vector2 placingBoundary = (Vector2) currentZone.center + (currentZone.size * 0.5f);

            Instantiate(
                placableNPCs[Random.Range(0, placableNPCs.Length)], 
                new Vector2(Random.Range(-placingBoundary.x, placingBoundary.x), Random.Range(-placingBoundary.y, placingBoundary.y)), 
                Quaternion.identity, 
                currentZone.transform
            );
        }
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            // set current phase
            currentPhase = 1;
            // initialize states
            DefaultState = new DefaultState(this, this);
            PlacingState = new PlacingState(this, this);
            // initialize FSM
            Initialize(DefaultState);
        }

        new void Update()
        {
            if (Time.timeScale <= 0f) return;
            base.Update();
        }

        new void FixedUpdate()
        {
            if (Time.timeScale <= 0f) return;
            base.FixedUpdate();
        }
        #endregion
    }
}

