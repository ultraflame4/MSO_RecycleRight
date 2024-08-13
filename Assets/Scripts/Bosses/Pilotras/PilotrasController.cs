using System.Collections;
using System.Linq;
using UnityEngine;
using Bosses.Pilotras.FSM;
using Interfaces;
using Patterns.FSM;
using Level;
using Level.Bins;
using NPC;
using Random = UnityEngine.Random;

namespace Bosses.Pilotras
{
    public class PilotrasController : StateMachine<PilotrasController>, IDamagable, IMovable
    {
        #region Inspector
        [field: Header("Data")]
        [field: SerializeField] public PilotrasData data { get; private set; }
        [field: SerializeField] public PilotrasBehaviourData behaviourData { get; private set; }

        [field: Header("Components")]
        [field: SerializeField] public PilotrasFireController fireController { get; private set; }

        [Header("Debug")]
        [SerializeField] int debugPhase = 2;
        [SerializeField] bool debugMode = false;
        [SerializeField] bool showGizmos = true;
        [SerializeField] LevelZone debug_zone;
        #endregion

        #region Private Fields
        Vector2 minBounds, maxBounds;
        #endregion

        #region States
        public DefaultState DefaultState { get; private set; }
        public PlacingState PlacingState { get; private set; }
        public BinDropState BinDropState { get; private set; }
        public PostBinDropStunState PostBinDropStunState { get; private set; }
        public ToppleState ToppleState { get; private set; }
        public MeteorShowerAttackState MeteorShowerAttackState { get; private set; }
        public PhaseChangeState PhaseChangeState { get; private set; }
        public DeathState DeathState { get; private set; }
        #endregion

        #region References
        public LevelManager levelManager => LevelManager.Instance;
        public LevelZone zone => levelManager == null ? null : levelManager.current_zone;
        #endregion

        #region Other Properties
        private float _health = 0f;
        public float Health
        {
            get { return _health; }
            set { _health = Mathf.Clamp(value, 0f, data.max_health); }
        }

        public int currentPhase { get; private set; } = 0;
        public float meteorAttackChance => behaviourData == null ? 0f : 
            Mathf.Clamp01(behaviourData.base_enter_attack_chance + (behaviourData.attack_chance_increase * (currentPhase - 1)));
        public float yPosTop => levelManager == null? 0f : 
            levelManager.current_zone.center.y + (levelManager.current_zone.size.y / 2f);
        #endregion

        #region Public Methods
        /// <summary>
        /// Spawns a random NPC from spawnable array
        /// </summary>
        /// <returns>NPC that was spawned</returns>
        public GameObject PlaceNPC(Vector3 position)
        {
            if (levelManager == null || data.currentPhaseNPCs == null || data.currentPhaseNPCs.Length <= 0)
                    return null;

            return Instantiate(
                data.currentPhaseNPCs[Random.Range(0, data.currentPhaseNPCs.Length)], 
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
            return new Vector2(Random.Range(minBounds.x, maxBounds.x), Random.Range(minBounds.y, maxBounds.y));
        }

        /// <summary>
        /// Update current number of NPCs in the scene
        /// </summary>
        public void UpdateNPCCount()
        {
            data.npcCount.Clear();
            if (LevelManager.Instance == null) return;
            data.recyclables = LevelManager.Instance.current_zone.GetComponentsInChildren<FSMRecyclableNPC>();
            if (data.recyclables == null || data.recyclables.Length <= 0) return;

            // count number of each recyclable
            foreach (FSMRecyclableNPC recyclable in data.recyclables)
            {
                RecyclableType type = recyclable.recyclableType;

                if (!data.npcCount.ContainsKey(type))
                    data.npcCount.Add(type, 1);
                else
                    data.npcCount[type]++;
            }
        }

        /// <summary>
        /// Handle changing phase
        /// </summary>
        public void HandlePhaseChange()
        {
            currentPhase++;
            LoadSpawnableNPCs();
            SpawnBins();
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

            if (obj != null) obj.transform.position = targetPosition;
        }
        #endregion

        #region Interface Methods
        public void Damage(float damage)
        {
            Health -= damage * data.damageTakenScale;
            if (Health > 0f) return;
            SwitchState(PhaseChangeState);
        }

        public bool CanMove()
        {
            return false;
        }
        #endregion

        #region Private Methods
        void LoadSpawnableNPCs()
        {
            int index = currentPhase - 1;

            if (index < 0 || data.spawnable_npcs == null || data.spawnable_npcs.Length == 0 || 
                index >= data.spawnable_npcs.Length) 
                    return;

            if (index == 0)
                data.currentPhaseNPCs = data.spawnable_npcs[0].gameObjects;
            else
                data.currentPhaseNPCs = data.currentPhaseNPCs
                    .Concat(data.spawnable_npcs[index].gameObjects)
                    .Where(x => x != null)
                    .ToArray();
        }

        void SpawnBins()
        {
            int index = currentPhase - 1;

            if (index < 0 || data.spawnable_bins == null || data.spawnable_bins.Length == 0 || 
                index >= data.spawnable_bins.Length) 
                    return;

            if (index == 0)
                data.spawnedBins = data.spawnable_bins[0].gameObjects
                    .Select(x => Instantiate(x, behaviourData.inactive_bins))
                    .Select(x => x.GetComponent<RecyclingBin>())
                    .Where(x => x != null)
                    .ToArray();
            else
                data.spawnedBins = data.spawnedBins
                    .Concat(data.spawnable_bins[index].gameObjects
                        .Select(x => Instantiate(x, behaviourData.inactive_bins))
                        .Select(x => x.GetComponent<RecyclingBin>())
                        .Where(x => x != null))
                    .ToArray();
            
            foreach (RecyclingBin bin in data.spawnedBins)
            {
                if (data.binScore.ContainsKey(bin.recyclableType)) return;
                data.binScore.Add(bin.recyclableType, 0);
            }
        }

        void CalculateBounds(LevelZone zone)
        {
            minBounds.x = zone.center.x - (zone.size.x / 2f);
            minBounds.y = zone.center.y - (zone.size.y / 2f);
            maxBounds.x = zone.center.x + (zone.size.x / 2f);
            maxBounds.y = zone.center.y + (zone.size.y / 2f);
            maxBounds.y = maxBounds.y - (maxBounds.y - transform.position.y) - data.y_offset;
        }
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            // reset variables
            Health = data.max_health;
            currentPhase = 0;
            // initialize first phase
            HandlePhaseChange();

            // initialize states
            DefaultState = new DefaultState(this, this);
            PlacingState = new PlacingState(this, this);
            PostBinDropStunState = new PostBinDropStunState(this, this);
            BinDropState = new BinDropState(this, this);
            ToppleState = new ToppleState(this, this);
            MeteorShowerAttackState = new MeteorShowerAttackState(this, this);
            PhaseChangeState = new PhaseChangeState(this, this);
            DeathState = new DeathState(this, this);
            // initialize FSM
            Initialize(DefaultState);

            // handle debug
            if (!debugMode) return;
            // debug phases
            for (int i = 0; i < debugPhase - currentPhase; i++)
            {
                HandlePhaseChange();
            }
        }

        void Start()
        {
            CalculateBounds(zone);
            if (fireController == null) return;
            fireController.minBounds = minBounds;
            fireController.maxBounds = maxBounds;
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

        #region Gizmos
        void OnDrawGizmosSelected() 
        {
            if (!showGizmos) return;

            if (BinDropState != null) 
                BinDropState.DrawDebug();
            
            if (debug_zone == null) return;
            CalculateBounds(debug_zone);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(minBounds, 0.5f);
            Gizmos.DrawSphere(maxBounds, 0.5f);
        }
        #endregion
    }
}

