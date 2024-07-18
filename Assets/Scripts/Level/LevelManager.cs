using System;
using System.Collections;
using UnityEngine;
using Level.Bins;

namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        [field: Header("Zones")]
        [field: SerializeField, Tooltip("The zones that the player can switch between. This is automatically retrieved at runtime.")]
        public LevelZone[] zones { get; private set; }

        [field: SerializeField, Tooltip("The index of the current zone.")]
        public int current_zone_index { get; private set; } = 0;

        [Header("Level Management")]
        [Tooltip("Delay before changing to the next zone.")]
        [SerializeField] float zoneChangeDelay = 2.5f;
        [SerializeField] string binTag = "Bin";
        [Tooltip("Controls whether to update zones automatically.")]
        [SerializeField] bool updateZone = true;

        [field: Header("References")]
        [field: SerializeField, Tooltip("The level camera")]
        public new LevelCamera camera { get; private set; }
        public bool debug_move_to_current_zone = false;

        public LevelZone current_zone => zones[current_zone_index];
        public RecyclingBin[][] Bins { get; private set; }

        /// <summary>
        /// The instance of the LevelManager in the scene. If there is no instance, it will be null;
        /// </summary>
        public static LevelManager _instance {get; private set;} = null;
        /// <summary>
        /// Returns the instance of the LevelManager in the scene. If there is no instance, it will throw an exception.
        /// </summary>
        public static LevelManager Instance {
            get {
                if (_instance == null)
                {
                    throw new NullReferenceException("There is no LevelManager in the scene!");
                }
                return _instance;
            }
        }

        Coroutine coroutine_zone_change;

        public event Action<LevelZone> ZoneChanged;

        private void Awake() {
            
            if (_instance != null)
            {
                Debug.LogWarning("Existing LevelManager found! Will replace existing instance! If this caused by scene loading, ignore.");
            }
            _instance = this;
        }

        public void Start()
        {
            zones = transform.GetComponentsInChildren<LevelZone>();
            MoveToZone(0);
            // get references to recycling bins, and disable other zones
            Bins = new RecyclingBin[zones.Length][];
            for (int i = 0; i < zones.Length; i++)
            {
                Bins[i] = zones[i].GetComponentsInChildren<RecyclingBin>();
                if (i == 0 || !updateZone) continue;
                SetZoneActive(false, i);
            }
        }

        // This is in late update because the check for zone completion should only be done after all the other logic has completed
        void LateUpdate()
        {
            if (!updateZone || zones == null ||
                zones[current_zone_index].transform.childCount > 
                Bins[current_zone_index].Length) 
                    return;
            
            // check for level completion
            if (current_zone_index >= (zones.Length - 1))
            {
                Debug.Log("Level Completed.");
                return;
            }

            if (coroutine_zone_change != null) return;
            coroutine_zone_change = StartCoroutine(DelayedZoneUpdate());
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

        /// <summary>
        /// Set the active of the zone
        /// </summary>
        /// <param name="active">Whether to activate or deactivate zone</param>
        /// <param name="index">Index of zone</param>
        public void SetZoneActive(bool active, int index)
        {
            if (zones == null || zones.Length <= index) return;
            foreach (Transform child in zones[index].transform)
            {
                if (child.gameObject.CompareTag(binTag)) continue;
                child.gameObject.SetActive(active);
            }
        }

        IEnumerator DelayedZoneUpdate()
        {
            yield return new WaitForSeconds(zoneChangeDelay);
            SetZoneActive(true, current_zone_index + 1);
            MoveToZone(current_zone_index + 1);
            coroutine_zone_change = null;
        }
    }
}
