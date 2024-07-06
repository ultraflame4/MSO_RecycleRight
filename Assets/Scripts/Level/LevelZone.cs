using UnityEngine;

namespace Level
{
    public class LevelZone : MonoBehaviour
    {
        [field: SerializeField]
        public Vector2 size { get; private set; } = new Vector2(20, 10);
        [field: SerializeField, Tooltip("How much to offset the zone. Should not affect camera positioning.")]
        public Vector2 offset { get; private set; } = Vector2.zero;
        [field: SerializeField, Tooltip("The buffer zone size. This is mainly used for NPC Ai Navigation")]
        public float buffer_zone_size { get; private set; } = 10f;

        [field: SerializeField, Tooltip("Determines where the player should start in the zone. This is the offset from the left edge of the zone.")]
        public float player_start_offset { get; private set; } = 1;
        public float player_startpos_x => center.x - size.x / 2 + player_start_offset;
        /// <summary>
        /// The center of the zone
        /// </summary>
        public Vector3 center=> transform.position + (Vector3)offset;
        /// <summary>
        /// The position the camera should target
        /// </summary>
        public Vector2 camera_target_pos=> transform.position;
        public ILevelEntity[] entities;

        private void Start()
        {
            entities = GetComponentsInChildren<ILevelEntity>();
        }

        public void StartZone()
        {
            Debug.Log("Zone started");
            foreach (var enemy in entities)
            {
                enemy.OnZoneStart();
            }
        }
        public bool PositionWithinZone(Vector3 position)
        {
            bool x = position.x >= center.x - size.x / 2 && position.x <= center.x + size.x / 2;
            bool y = position.y >= center.y - size.y / 2 && position.y <= center.y + size.y / 2;
            return x && y;
        }

        public float DistanceFromEdgeX(Vector3 position)
        {
            float leftEdge = center.x - size.x / 2;
            float rightEdge = center.x + size.x / 2;
            return Mathf.Min(position.x - leftEdge, rightEdge - position.x);
        }
        public float DistanceFromEdgeY(Vector3 position)
        {
            float bottomEdge = center.y - size.y / 2;
            float topEdge = center.y + size.y / 2;
            return Mathf.Min(position.y - bottomEdge, topEdge - position.y);
        }

        public float DistanceFromEdge(Vector3 position)
        {
            return Mathf.Min(DistanceFromEdgeX(position), DistanceFromEdgeY(position));
        }

        public bool PositionWithinBufferZone(Vector3 position)
        {
            
            return DistanceFromEdge(position) < buffer_zone_size;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(center, size);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(center, size - Vector2.one * buffer_zone_size * 2);
            Gizmos.color = Color.cyan * .1f;
            Gizmos.DrawCube(center, size);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector2(player_startpos_x, center.y - (size.y) / 2), new Vector2(player_startpos_x, center.y + size.y / 2));
        }
    }
}
