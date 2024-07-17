using System;
using UnityEngine;

namespace NPC
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Navigation : MonoBehaviour
    {

        [Tooltip("At what distance from the destination should this game object stop")]
        public float stop_distance = 0.1f;
        [Tooltip("The distance from the destination at which this game object start slowing down."), Range(0.00001f,1f)]
        public float slow_distance = 0.1f;
        [Tooltip("The movement speed")]
        public float move_speed = 100f;
        [Tooltip("Checks for target position overshooting and fixes it.")]
        public bool fix_overshoot = true;
        
        private Rigidbody2D rb;
        private Transform target;
        private Vector3? current_target_pos;

        public bool flipX {
            get {
                if (rb){
                    return rb.velocity.x > 0;
                }
                return false;
            }
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// True when has reached destination or has not target position;
        /// </summary>
        public bool reachedDestination
        {
            get
            {
                if (current_target_pos == null) return true;
                return (current_target_pos.Value - transform.position).sqrMagnitude < stop_distance * stop_distance;
            }
        }

        [EasyButtons.Button]
        public void ClearDestination(){
            target = null;
            current_target_pos = null;
        }

        public void SetDestination(Vector3 pos)
        {
            current_target_pos = pos;
        }

        public void StopVelocity(){
            rb.velocity = Vector3.zero;
        }

        public void SetDestination(Transform target)
        {
            this.target = target;
        }

        private void FixedUpdate()
        {
            if (target != null)
            {
                current_target_pos = target.position;
            }

            if (current_target_pos != null)
            {
                MoveToStep(current_target_pos.Value);
            }
        }

        private void DetectAndFixOvershoot(Vector3 target_pos, Vector3 current_dir){
            Vector3 next_pos = (Vector2)transform.position + rb.velocity * Time.deltaTime;
            Vector3 next_dir = target_pos - next_pos; // The direction from the next position to the target position
            // If the next position is in the opposite direction of the target position, then stop (to prevent overshooting)
            if (Vector3.Dot(next_dir, current_dir) < 0)
            {
                rb.velocity = Vector2.zero;
                transform.position = target_pos;
            }
        }

        private void MoveToStep(Vector3 target_pos)
        {
            Vector3 dir = target_pos - transform.position;
            Vector3 vel = dir.normalized * move_speed * Time.deltaTime;
            rb.velocity = vel * Mathf.Clamp01(dir.magnitude * (1/slow_distance));

            if (fix_overshoot) DetectAndFixOvershoot(target_pos,dir);

            if (reachedDestination) rb.velocity = Vector2.zero;
        }

        private void OnDrawGizmosSelected()
        {
            if (current_target_pos == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(current_target_pos.Value, stop_distance);
            Gizmos.DrawLine(transform.position, current_target_pos.Value);
        }
    }
}
