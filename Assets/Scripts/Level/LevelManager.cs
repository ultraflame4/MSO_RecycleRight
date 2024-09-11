using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        #region Unity Configuration
        [field: Header("Zones")]
        [field: SerializeField, Tooltip("The zones that the player can switch between. This is automatically retrieved at runtime.")]
        public LevelZone[] zones { get; private set; }

        [field: SerializeField, Tooltip("The index of the current zone.")]
        public int current_zone_index { get; private set; } = 0;

        [Header("Level Management")]
        [Tooltip("Delay before changing to the next zone.")]
        [SerializeField] float zoneChangeDelay = 2.5f;
        [SerializeField] string binTag = "Bin";
        [Tooltip("Controls whether to change zones automatically when they are completed."), FormerlySerializedAs("updateZone")]
        [SerializeField] bool autoChangeZone = true;

        [field: Header("References")]
        [field: SerializeField, Tooltip("The level camera")]
        public new LevelCamera camera { get; private set; }
        public bool debug_move_to_current_zone = false;
        [field: SerializeField, Tooltip("The level info for this level. When missing, some features may not work.")]
        public LevelInfo levelInfo { get; private set; }
        [SerializeField, Tooltip("The level end menu for this level. When missing, some features may not work.")]
        private LevelEndMenu levelEnd;
        #endregion

        #region Singleton
        /// <summary>
        /// The instance of the LevelManager in the scene. If there is no instance, it will be null;
        /// </summary>
        public static LevelManager _instance { get; private set; } = null;
        /// <summary>
        /// Returns the instance of the LevelManager in the scene. If there is no instance, it will throw an exception.
        /// </summary>
        public static LevelManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new NullReferenceException("There is no LevelManager in the scene!");
                }
                return _instance;
            }
        }
        private void Awake()
        {

            if (_instance != null)
            {
                Debug.LogWarning("Existing LevelManager found! Will replace existing instance! If this caused by scene loading, ignore.");
            }
            _instance = this;
        }

        #endregion

        Coroutine coroutine_zone_change;

        public LevelZone current_zone => zones[current_zone_index];
        public bool LevelEnded { get; private set; } = false;

        public event Action<LevelZone> ZoneChanged;

        public virtual void Start()
        {
            zones = transform.GetComponentsInChildren<LevelZone>();
            ChangeZone(0);
        }


        // This is in late update because the check for zone completion should only be done after all the other logic has completed
        void LateUpdate()
        {
            // Skip logic if the level has ended
            if (LevelEnded) return;
            if (zones == null) return;
            // Skip change zone and end level logic if autoChangeZone is disabled
            if (!autoChangeZone) return;
            // check if the current zone is complete, if not, skip
            if (!current_zone.zoneComplete) return;
            // check for level completion
            if (current_zone_index >= (zones.Length - 1))
            {
                EndLevel();
                return;
            }
            // prevent multiple zone changes, skip zone change if autoChangeZone is disabled
            if (coroutine_zone_change != null) return;
            coroutine_zone_change = StartCoroutine(NextZone_coroutine());
        }


        IEnumerator NextZone_coroutine()
        {
            yield return new WaitForSeconds(zoneChangeDelay);
            ChangeZone(current_zone_index + 1);
            coroutine_zone_change = null;
        }


        public void MoveToZone(int index)
        {
            current_zone_index = index;
            // call event when moving to new zone
            // camera.ClearBounding();
            ZoneChanged?.Invoke(current_zone);
            camera.pendingBoundsUpdate=true;
        }

        private void OnValidate()
        {
            if (debug_move_to_current_zone)
            {
                debug_move_to_current_zone = false;
                MoveToZone(current_zone_index);
            }
        }

        public void ChangeZone(int new_zone_index)
        {
            current_zone_index = new_zone_index;
            MoveToZone(new_zone_index);
            if (!autoChangeZone) return;
            
            // disable all other zones
            foreach (var zone in zones)
            {
                if (zone == current_zone) continue;
                zone.DeactiveZone();
            }

            current_zone.ActivateZone();
        }

        public virtual float GetCurrentScore()
        {
            return zones.Sum(x => x.bins.Sum(y => y.Score));
        }

        public void EndLevel(bool fail = false)
        {
            if (LevelEnded) return;
            if (levelInfo == null)
            {
                Debug.LogWarning("LevelInfo is missing. Cannot end level.");
                return;
            }

            if (levelEnd == null)
            {
                Debug.LogWarning("LevelEndMenu is missing. Cannot end level.");
                return;
            }
            LevelEnded = true;
            levelEnd.ShowEndScreen(GetCurrentScore(), levelInfo.Data.maxScore, fail);
        }
    }
}
