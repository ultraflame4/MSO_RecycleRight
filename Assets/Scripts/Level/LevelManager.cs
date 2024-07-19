using System;
using System.Collections;
using UnityEngine;
using Level.Bins;
using System.Linq;
using System.Xml.Serialization;
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
        [SerializeField, Tooltip("The level info for this level. When missing, some features may not work.")]
        private LevelInfo levelInfo;
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


        public LevelZone current_zone => zones[current_zone_index];

        Coroutine coroutine_zone_change;
        private bool levelEnded = false;

        public event Action<LevelZone> ZoneChanged;

        public void Start()
        {
            zones = transform.GetComponentsInChildren<LevelZone>();


            ChangeZone(0);
        }


        // This is in late update because the check for zone completion should only be done after all the other logic has completed
        void LateUpdate()
        {
            // Skip logic if the level has ended
            if (levelEnded) return;
            if (zones == null) return;
            // check if the current zone is complete, if not, skip
            if (!current_zone.zoneComplete) return;
            // check for level completion
            if (current_zone_index >= (zones.Length - 1))
            {
                Debug.Log("Level Completed.");
                EndLevel();
                return;
            }
            // prevent multiple zone changes, skip zone change if autoChangeZone is disabled
            if (coroutine_zone_change != null || !autoChangeZone) return;
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
            camera.zone_position = current_zone.transform.position;
            // call event when moving to new zone
            ZoneChanged?.Invoke(current_zone);
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
            current_zone.RefereshEntities();
            // disable all other zones
            foreach (var zone in zones)
            {
                if (zone == current_zone) continue;
                zone.DeactiveZone();
            }
            current_zone.ActivateZone();
            MoveToZone(new_zone_index);
        }

        public float GetCurrentScore()
        {
            return zones.Sum(x => x.bins.Sum(y => y.Score));
        }

        public void EndLevel()
        {
            if (levelEnded) return;
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
            levelEnded = true;
            levelEnd.ShowEndScreen(GetCurrentScore(), levelInfo.Data.maxScore);
        }
    }
}
