using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Navigation : MonoBehaviour
{
    private Transform target;     
    private Vector3? current_target_pos;


    public void SetDestination(Vector3 pos)
    {
        current_target_pos = pos;
    }

    public void SetDestination(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (target == null)
        {
            current_target_pos = target.position;
        }

        if (current_target_pos != null)
        {
            MoveToStep(current_target_pos.Value);
        }
    }

    private void MoveToStep(Vector3 pos)
    {
        // todo change this later
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
    }
}