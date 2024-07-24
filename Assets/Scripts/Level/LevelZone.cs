using System.Linq;
using Level.Bins;
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
        public Vector2 player_startpos_offset { get; private set; } = new Vector2(2, 0);
        public Vector2 player_startpos => new Vector2(
            center.x - size.x / 2 + player_startpos_offset.x,
            center.y + player_startpos_offset.y
        );
        /// <summary>
        /// The center of the zone
        /// </summary>
        public Vector3 center=> transform.position + (Vector3)offset;
        /// <summary>
        /// The position the camera should target
        /// </summary>
        public Vector2 camera_target_pos=> transform.position;
        public bool zoneComplete {get; private set;}
        public ILevelEntity[] entities;
        public RecyclingBin[] bins;
        
        private void Start()
        {
            entities = GetComponentsInChildren<ILevelEntity>();
            bins = GetComponentsInChildren<RecyclingBin>();
        }

        public void RefreshEntities()
        {
            entities = GetComponentsInChildren<ILevelEntity>(true);
        }

        /// <summary>
        /// Activates the zone. This is called when the player enters the zone.
        /// </summary>
        public void ActivateZone()
        {
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
            RefreshEntities();
            foreach (var entity in entities)
            {
                // Some entities may be destroyed before the zone is deactivated (i.e enemies)
                if (entity != null) entity.OnZoneEnd();
            }
        }

        /// <summary>
        /// Check if the zone is finished. This is done by checking if there are any trash items left in the zone.
        /// 
        /// This method is expensive and should not be called frequently. It is recommended to use zoneComplete property instead.
        /// </summary>
        /// <returns></returns>
        public bool CheckZoneFinished(){
            return GetComponentsInChildren<IBinTrashItem>().Length < 1;
        }

        private void OnTransformChildrenChanged() {
            zoneComplete = CheckZoneFinished();
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
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(center, size);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(center, size - Vector2.one * buffer_zone_size * 2);
            Gizmos.color = Color.cyan * .1f;
            Gizmos.DrawCube(center, size);

            Gizmos.color = Color.green * .5f;
            Gizmos.DrawSphere(player_startpos,.25f);
        }
    }
}
