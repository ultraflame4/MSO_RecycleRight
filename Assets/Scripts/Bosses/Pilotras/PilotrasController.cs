using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Bosses.Pilotras.FSM;
using Interfaces;
using Patterns.FSM;
using Level;
using Random = UnityEngine.Random;

namespace Bosses.Pilotras
{
    public class PilotrasController : StateMachine<PilotrasController>, IDamagable
    {
        #region Data
        [field: SerializeField] public PilotrasData data { get; private set; }
        [field: SerializeField] public PilotrasBehaviourData behaviourData { get; private set; }
        #endregion

        #region States
        public DefaultState DefaultState { get; private set; }
        public PlacingState PlacingState { get; private set; }
        public BinDropState BinDropState { get; private set; }
        #endregion

        #region References
        public LevelManager levelManager => LevelManager.Instance;
        #endregion

        #region Other Properties
        private float _health = 0f;
        public float Health
        {
            get { return _health; }
            set { _health = Mathf.Clamp(value, 0f, data.max_health); }
        }

        public int currentPhase { get; private set; } = 0;
        #endregion

        #region Events
        public event Action EndLevel;
        #endregion

        #region Public Methods
        /// <summary>
        /// Spawns a random NPC from spawnable array
        /// </summary>
        /// <returns>NPC that was spawned</returns>
        public GameObject PlaceNPC(Vector3 position)
        {
            if (levelManager == null || data.spawnable_npcs == null || data.spawnable_npcs.Length <= 0 ||
                data.spawnable_npcs[0].gameObjects == null || data.spawnable_npcs[0].gameObjects.Length <= 0)
                    return null;
            
            GameObject[] placableNPCs = data.spawnable_npcs[0].gameObjects;
            
            for (int i = 1; i < currentPhase; i++)
            {
                if (data.spawnable_npcs.Length <= i) break;

                placableNPCs = placableNPCs
                    .Concat(data.spawnable_npcs[i].gameObjects)
                    .Where(x => x != null)
                    .ToArray();
            }

            if (placableNPCs == null || placableNPCs.Length <= 0) return null;

            return Instantiate(
                placableNPCs[Random.Range(0, placableNPCs.Length)], 
                position, 
                Quaternion.identity, 
                levelManager.current_zone.transform
            );
        }

        /// <summary>
        /// Returns a random position within the bounds of the current zone
        /// </summary>
        /// <returns>Generated position</returns>
        public Vector2 GetRandomPositionInZone()
        {
            LevelZone currentZone = levelManager.current_zone;
            Vector2 boundary = (Vector2) currentZone.center + (currentZone.size * 0.5f);
            return new Vector2(Random.Range(-boundary.x, boundary.x), Random.Range(-boundary.y, boundary.y));
        }
        #endregion

        #region Public Coroutines
        /// <summary>
        /// Moves an object from its current positin to the target position in a set duration
        /// </summary>
        /// <param name="duration">Duration of movement</param>
        /// <param name="obj">Object to move</param>
        /// <param name="targetPosition">Final end position of movement</param>
        public IEnumerator Throw(float duration, GameObject obj, Vector3 targetPosition)
        {
            float timeElasped = 0f;
            Vector3 originalPosition = obj.transform.position;

            while (timeElasped < duration)
            {
                if (obj == null) break;
                obj.transform.position = Vector3.Lerp(originalPosition, targetPosition, timeElasped / duration);
                timeElasped += Time.deltaTime;
                yield return timeElasped;
            }
        }
        #endregion

        #region Interface Methods
        public void Damage(float damage)
        {
            Health -= damage;
            if (Health > 0f) return;

            if (currentPhase >= data.number_of_phases) 
            {
                // handle death state
                EndLevel?.Invoke();
                return;
            }

            Health = data.max_health;
            currentPhase++;
        }
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            // reset variables
            Health = data.max_health;
            currentPhase = 1;
            // initialize states
            DefaultState = new DefaultState(this, this);
            PlacingState = new PlacingState(this, this);
            BinDropState = new BinDropState(this, this);
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

