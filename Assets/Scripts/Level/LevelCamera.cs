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

        public float intensity = 15f;
        public float frequency = 0.21f;


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
        float og_freq, og_amp;
        IEnumerator ShakeCamera_Coroutine(CinemachineBasicMultiChannelPerlin cameraMultiChannelPerlin, float time, float? overrideIntensity = null)
        {
            if (cameraMultiChannelPerlin != null)
            {
                og_freq = cameraMultiChannelPerlin.m_FrequencyGain;
                og_amp = cameraMultiChannelPerlin.m_AmplitudeGain;

                cameraMultiChannelPerlin.m_AmplitudeGain = overrideIntensity.GetValueOrDefault(intensity);
                cameraMultiChannelPerlin.m_FrequencyGain = frequency;
                yield return new WaitForSeconds(time);
                ResetCameraShake(cameraMultiChannelPerlin);
            }

            camera_shake_coroutine = null;
        }

        public void ShakeCamera(float time, float? overrideIntensity = null)
        {
            var brain = GetComponentInChildren<CinemachineBrain>();
            var virtualCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
            CinemachineBasicMultiChannelPerlin cameraMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if (camera_shake_coroutine != null) ResetCameraShake(cameraMultiChannelPerlin);
            camera_shake_coroutine = StartCoroutine(ShakeCamera_Coroutine(cameraMultiChannelPerlin, time, overrideIntensity));
        }

        void ResetCameraShake(CinemachineBasicMultiChannelPerlin cameraMultiChannelPerlin)
        {
            if (cameraMultiChannelPerlin == null) return;
            StopCoroutine(camera_shake_coroutine);
            cameraMultiChannelPerlin.m_FrequencyGain = og_freq;
            cameraMultiChannelPerlin.m_AmplitudeGain = og_amp;
        }

        [EasyButtons.Button]
        void ShakeCamera_Inspector(float time, float overrideIntensity)
        {
            ShakeCamera(time, overrideIntensity);
        }
    }
}
