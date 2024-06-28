using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Navigation : MonoBehaviour
{

    [Tooltip("When distance between target & this game object is less than stop_distance, it will count as reached destination.")]
    public float stop_distance = 0.1f;
    [Tooltip("When distance to start slowing down"), Range(0.00001f,1f)]
    public float slow_distance = 0.1f;
    [Tooltip("The movement speed")]
    public float move_speed = 100f;

    private Rigidbody2D rb;
    private Transform target;
    private Vector3? current_target_pos;

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

    public void ClearDestination(){
        target = null;
        current_target_pos = null;
    }

    public void SetDestination(Vector3 pos)
    {
        current_target_pos = pos;
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

    private void MoveToStep(Vector3 target_pos)
    {
        Vector3 dir = target_pos - transform.position;
        Vector3 vel = dir.normalized * move_speed * Time.deltaTime;
        rb.velocity = vel * Mathf.Clamp01(dir.magnitude * (1/slow_distance));
        
        Vector3 next_pos = transform.position + vel * Time.deltaTime;
        Vector3 next_dir = target_pos - next_pos; // The direction from the next position to the target position
        // If the next position is in the opposite direction of the target position, then stop (to prevent overshooting)
        if (Vector3.Dot(next_dir, dir) < 0)
        {
            rb.velocity = Vector2.zero;
            transform.position = target_pos;
        }

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