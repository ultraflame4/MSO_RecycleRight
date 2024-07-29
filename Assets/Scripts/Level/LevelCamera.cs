using System.Collections;
using Cinemachine;
using Player;
using UnityEngine;

namespace Level
{
    public class LevelCamera : MonoBehaviour
    {
        [SerializeField]
        private CinemachineConfiner2D confiner2D;
        public bool pendingBoundsUpdate;

        public void UpdateBoundingShape()
        {
            confiner2D.m_BoundingShape2D = LevelManager.Instance.current_zone.boundary;

        }

        private void Update()
        {
            if (pendingBoundsUpdate)
            {
                var player = PlayerController.Instance;
                // Skip if player not found
                if (!player) return;
                var playerWithinZone = LevelManager.Instance?.current_zone.PositionWithinZone(player.transform.position);
                // Skip if player not in zone
                if (playerWithinZone != true) return;
                // If player is in current zone,
                pendingBoundsUpdate = false;
                StartCoroutine(Delayed_UpdateBoundingShape());
            }
        }

        IEnumerator Delayed_UpdateBoundingShape()
        {
            yield return new WaitForSeconds(0.5f);
            UpdateBoundingShape();
        }

    }
}
