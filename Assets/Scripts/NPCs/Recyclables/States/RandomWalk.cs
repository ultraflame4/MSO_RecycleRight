using UnityEngine;

public class RandomWalk : BaseRecyclableState
{
    public float walk_distance = 2f;
    public float current_direction_strength = .1f;
    public float rand_direction_strength = .9f;


    private Vector3 current_direction = Vector3.zero;
    public RandomWalk(StateMachine<FSMRecyclableNPC> fsm, FSMRecyclableNPC character) : base(fsm, character)
    {
    }

    public override void LogicUpdate()
    {
        if (navigation.reachedDestination)
        {
            // Select new rand direction;
            Vector3 randDir = new Vector3(Random.value, Random.value, 0).normalized;

            current_direction = (randDir * rand_direction_strength) + (current_direction * current_direction_strength);
            current_direction.Normalize();
            Debug.Log($"Dir {current_direction}, mag {current_direction.magnitude}");
            navigation.SetDestination(transform.position + current_direction * walk_distance);
        }

        Debug.DrawRay(transform.position, current_direction);
    }

}