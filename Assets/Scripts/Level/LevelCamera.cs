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
        private void Update()
        {
            // if (lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
            // {
            //     lastScreenSize = new Vector2(Screen.width, Screen.height);
            //     Adjust();
            // }

            // Vector3 target_position;
            // if (allowPeeking)
            // {
            //     target_position = Vector3.Lerp(PlayerController.Instance.transform.position, zone_position, 0.8f);

            // }
            // else
            // {
            //     target_position = zone_position;
            // }
            // target_position.z = camera.transform.position.z;
            // transform.position = Vector3.SmoothDamp(transform.position, target_position, ref velocity, smoothTime);
            confiner2D.m_BoundingShape2D = LevelManager.Instance.current_zone.boundary;
        }
    }
}
