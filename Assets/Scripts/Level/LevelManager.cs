using System;
using System.Collections;
using UnityEngine;

namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        [field: SerializeField, Tooltip("The zones that the player can switch between. This is automatically retrieved at runtime.")]
        public LevelZone[] zones { get; private set; }

        [field: SerializeField, Tooltip("The index of the current zone.")]
        public int current_zone_index { get; private set; } = 0;

        [field: Header("References")]
        [field: SerializeField, Tooltip("The level camera")]
        public new LevelCamera camera { get; private set; }
        public bool debug_move_to_current_zone = false;

        public LevelZone current_zone => zones[current_zone_index];

        public event Action<LevelZone> ZoneChanged;

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

        private void Awake() {
            if (_instance == null)
            {
                _instance = this;
            }
            else{
                Debug.LogWarning("There are multiple LevelManagers in the scene! This is not allowed!");
            }
        }

        public void Start()
        {
            
            zones = transform.GetComponentsInChildren<LevelZone>();
            MoveToZone(0);
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
    }
}
