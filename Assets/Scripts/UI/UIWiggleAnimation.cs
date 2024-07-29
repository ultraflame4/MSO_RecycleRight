using System.Collections;
using UnityEngine;

namespace UI
{
    public class UIWiggleAnimation : MonoBehaviour
    {
        [SerializeField] Vector2 cooldown;
        [SerializeField] Vector2 duration;
        [SerializeField] float magnitude = 15f;
        [SerializeField] Axis axis = Axis.Y;
        enum Axis
        {
            X, Y, Z
        }

        float originalRotation;
        Vector2 boundaries;
        Vector3 actualAxis;

        // Start is called before the first frame update
        void Start()
        {
            switch (axis)
            {
                case Axis.X:
                    actualAxis = Vector3.forward;
                    originalRotation = transform.rotation.eulerAngles.x;
                    break;
                case Axis.Y:
                    actualAxis = Vector3.up;
                    originalRotation = transform.rotation.eulerAngles.y;
                    break;
                case Axis.Z:
                    actualAxis = Vector3.right;
                    originalRotation = transform.rotation.eulerAngles.z;
                    break;
                default:
                    actualAxis = Vector3.zero;
                    originalRotation = 0f;
                    break;
            }

            boundaries = new Vector2(originalRotation - magnitude, originalRotation + magnitude);
            StartCoroutine(WaitForCooldown());
        }

        IEnumerator WaitForCooldown()
        {
            yield return new WaitForSeconds(Random.Range(cooldown.x, cooldown.y));
            StartCoroutine(Wiggle());
        }

        IEnumerator Wiggle()
        {
            float timeElasped = 0f;
            float maxTime = Random.Range(duration.x, duration.y);
            float boundary = Random.Range(boundaries.x, originalRotation);

            while (timeElasped < (maxTime / 3f))
            {
                timeElasped += Time.deltaTime;
                Rotate(boundary);
                yield return timeElasped;
            }

            boundary = Random.Range(originalRotation, boundaries.y);

            while (timeElasped < (2f * (maxTime / 3f)))
            {
                timeElasped += Time.deltaTime;
                Rotate(boundary);
                yield return timeElasped;
            }

            while (timeElasped < maxTime)
            {
                timeElasped += Time.deltaTime;
                Rotate(originalRotation);
                yield return timeElasped;
            }

            transform.rotation = Quaternion.Euler(actualAxis * originalRotation);
            StartCoroutine(WaitForCooldown());
        }

        void Rotate(float targetAngle)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.Euler(actualAxis * targetAngle), 
                Time.deltaTime);
        }
    }
}
