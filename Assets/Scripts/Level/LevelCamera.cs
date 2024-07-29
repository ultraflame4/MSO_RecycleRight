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

        [Tooltip("The target position for the camera.")]
        public Vector3 zone_position;
        [Tooltip("Camera damping")]
        public float smoothTime = 0.3f;

        [Tooltip("Target camera aspect ratio (w/h). Will do funny stuff to the camera")]
        public Vector2 aspect = new Vector2(16, 9);

        public float aspect_ratio => aspect.x / aspect.y;

        private Vector3 velocity = Vector3.zero;

        private Vector2 lastScreenSize = Vector2.zero;
        [Tooltip("Make the camera lerp between the player and the zone position. This is an experimental solution to reveal zone areas covered by the UI.")]
        public bool allowPeeking = false;


        public bool pendingBoundsUpdate;
        private void Start()
        {
            Adjust();

        }

        private void Adjust()
        {
            // // reference https://www.youtube.com/watch?v=PClWqhfQlpU

            // float current_aspect = (float)Screen.width / Screen.height;
            // float scaleHeight = current_aspect / aspect_ratio;

            // if (scaleHeight < 1)
            // {
            //     // add pillarbox
            //     Rect rect = camera.rect;
            //     rect.width = 1;
            //     rect.height = scaleHeight;
            //     rect.x = 0;
            //     rect.y = (1 - scaleHeight) / 2;
            //     camera.rect = rect;
            // }
            // else
            // {
            //     // add letterbox
            //     float scaleWidth = 1 / scaleHeight;
            //     Rect rect = camera.rect;
            //     rect.width = scaleWidth;
            //     rect.height = 1;
            //     rect.x = (1 - scaleWidth) / 2;
            //     rect.y = 0;
            //     camera.rect = rect;
            // }

        }

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
