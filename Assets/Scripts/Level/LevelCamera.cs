using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    [field: SerializeField, Tooltip("The camera component.")]
    public Camera camera { get; private set; }

    [Tooltip("The target position for the camera.")]
    public Vector3 target_position;
    [Tooltip("Camera damping")]
    public float smoothTime = 0.3f;

    private Vector3 velocity;

    private void Update()
    {
        target_position.z = camera.transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, target_position, ref velocity, smoothTime);
    }
}