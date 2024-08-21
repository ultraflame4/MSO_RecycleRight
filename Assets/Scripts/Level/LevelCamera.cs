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
        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;
        public bool pendingBoundsUpdate;

        public float intensity = 15f;


        public void UpdateBoundingShape()
        {
            confiner2D.m_BoundingShape2D = LevelManager.Instance.current_zone.boundary;

        }

        private void Update()
        {
            if (pendingBoundsUpdate)
            {
                var player = PlayerController._instance;
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

        Coroutine camera_shake_coroutine;
        public IEnumerator ShakeCamera_Coroutine(float time, float? overrideIntensity = null)
        {
            CinemachineBasicMultiChannelPerlin cameraMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cameraMultiChannelPerlin.m_AmplitudeGain = overrideIntensity.GetValueOrDefault(intensity);
            yield return new WaitForSeconds(time);
            cameraMultiChannelPerlin.m_AmplitudeGain = 0;
        }


        public void ShakeCamera(float time, float? overrideIntensity = null)
        {
            if (camera_shake_coroutine != null)
            {
                StopCoroutine(camera_shake_coroutine);
            }
            camera_shake_coroutine = StartCoroutine(ShakeCamera_Coroutine(time, overrideIntensity));
        }

        [EasyButtons.Button]
        void ShakeCamera_Inspector(float time, float overrideIntensity)
        {
            if (camera_shake_coroutine != null)
            {
                StopCoroutine(camera_shake_coroutine);
            }
            camera_shake_coroutine = StartCoroutine(ShakeCamera_Coroutine(time, overrideIntensity));
        }


    }
}
