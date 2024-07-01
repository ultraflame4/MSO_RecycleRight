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

        public event Action<LevelManager, LevelZone> ZoneChanged;

        public void Start()
        {
            zones = transform.GetComponentsInChildren<LevelZone>();
            MoveToZone(0);
        }

        public void MoveToZone(int index)
        {
            current_zone_index = index;
            camera.target_position = current_zone.transform.position;
            // call event when moving to new zone
            ZoneChanged?.Invoke(this, current_zone);
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
