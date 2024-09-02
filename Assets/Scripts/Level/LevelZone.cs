using System.Collections.Generic;
using System.Linq;
using System.Text;
using Level.Bins;
using UnityEngine;

namespace Level
{
    [RequireComponent(typeof(PolygonCollider2D), typeof(EdgeCollider2D))]
    public class LevelZone : MonoBehaviour
    {
        [field: SerializeField]
        public Vector2 size { get; private set; } = new Vector2(20, 10);
        [field: SerializeField, Tooltip("How much to offset the zone. Should not affect camera positioning.")]
        public Vector2 offset { get; private set; } = Vector2.zero;
        [field: SerializeField, Tooltip("The buffer zone size. This is mainly used for NPC Ai Navigation")]
        public float peek_extend { get; private set; } = 1.5f;
        [field: SerializeField, Tooltip("How far can the camera peek outside the zone. (Defines CinemachineConfiner boundary)")]
        public float buffer_zone_size { get; private set; } = 10f;

        [field: SerializeField, Tooltip("Determines where the player should start in the zone. This is the offset from the left edge of the zone.")]
        public Vector2 player_startpos_offset { get; private set; } = new Vector2(2, 0);
        public Vector2 player_startpos => new Vector2(
            center.x - size.x / 2 + player_startpos_offset.x,
            center.y + player_startpos_offset.y
        );
        /// <summary>
        /// The center of the zone
        /// </summary>
        public Vector3 center => transform.position + (Vector3)offset;
        /// <summary>
        /// The position the camera should target
        /// </summary>
        public Vector2 camera_target_pos => transform.position;
        public bool zoneComplete { get; private set; }
        public ILevelEntity[] entities;
        public RecyclingBin[] bins;
        [field: SerializeField]
        public PolygonCollider2D boundary { get; private set; }
        public EdgeCollider2D edgeCollider { get; private set; }

        [Header("Debug draw")]
        public bool debug_drawLevelZone = true;
        public bool debug_drawBufferZone = true;
        public bool debug_drawPlayerStartPos = true;
        const float edge_diameter = 2f;

        private void Awake()
        {
            entities = GetComponentsInChildren<ILevelEntity>();
            UpdateBinsArray();
            GenerateBoundaries();
            GenerateBoundaryColliders();
        }

        private void LateUpdate()
        {
            if (zoneComplete || !edgeCollider.enabled) return;
            zoneComplete = CheckZoneFinished();
        }

        /// <summary>
        /// Check if the zone is finished. This is done by checking if there are any trash items left in the zone.
        /// 
        /// This method is expensive and should not be called frequently. It is recommended to use zoneComplete property instead.
        /// </summary>
        /// <returns></returns>
        public bool CheckZoneFinished()
        {
            entities = GetComponentsInChildren<ILevelEntity>();
            return entities.Length < 1;
        }

        /// <summary>
        /// Update references to bins in the zone
        /// </summary>
        public void UpdateBinsArray()
        {
            bins = GetComponentsInChildren<RecyclingBin>(true);
        }

        /// <summary>
        /// Activates the zone. This is called when the player enters the zone.
        /// </summary>
        public void ActivateZone()
        {
            edgeCollider.enabled = true;
            if (entities == null) return;
            foreach (var entity in entities)
            {
                entity.OnZoneStart();
            }
        }

        /// <summary>
        /// Deactivates the zone. This is called when the player leaves the zone.
        /// </summary>
        public void DeactiveZone()
        {
            edgeCollider.enabled = false;
            var activeEntities = GetComponentsInChildren<ILevelEntity>();
            foreach (var entity in activeEntities)
            {
                // Some entities may be destroyed before the zone is deactivated (i.e enemies)
                if (entity != null) entity.OnZoneEnd();
            }
        }

        #region Zone Bounds
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
        #endregion


        [EasyButtons.Button]
        public void GenerateBoundaries()
        {
            if (boundary == null)
            {
                boundary = GetComponent<PolygonCollider2D>();
            }
            boundary.isTrigger = true;
            var boundSize = size + Vector2.one * peek_extend * 2;
            var top_right = boundSize / 2;
            var top_left = new Vector2(-boundSize.x, boundSize.y) / 2;
            var bottom_left = new Vector2(-boundSize.x, -boundSize.y) / 2;
            var bottom_right = new Vector2(boundSize.x, -boundSize.y) / 2;

            boundary.SetPath(0, new Vector2[] { top_right, top_left, bottom_left, bottom_right });
        }


        [EasyButtons.Button]
        public void GenerateBoundaryColliders()
        {
            if (edgeCollider == null)
            {
                edgeCollider = GetComponent<EdgeCollider2D>();
                if (edgeCollider == null){
                    edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
                }
            }
            var boundSize = size;
            var top_right = new Vector2(boundSize.x + edge_diameter, boundSize.y + edge_diameter) / 2;
            var top_left = new Vector2(-boundSize.x - edge_diameter, boundSize.y + edge_diameter) / 2;
            var bottom_left = new Vector2(-boundSize.x - edge_diameter, -boundSize.y - edge_diameter) / 2;
            var bottom_right = new Vector2(boundSize.x + edge_diameter, -boundSize.y - edge_diameter) / 2;
            edgeCollider.edgeRadius = edge_diameter / 2;
            edgeCollider.SetPoints(new List<Vector2>(){
                top_right,top_left,bottom_left,bottom_right,top_right
            });
        }

        private void OnValidate()
        {
            GenerateBoundaries();
        }

        public void OnDrawGizmos()
        {
            if (debug_drawLevelZone)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(center, size);
            }

            if (debug_drawBufferZone)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(center, size - Vector2.one * buffer_zone_size * 2);
            }

            Gizmos.color = Color.green * .5f;
            Gizmos.DrawSphere(player_startpos, .25f);
        }
    }
}
