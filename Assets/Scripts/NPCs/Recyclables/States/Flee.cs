using UnityEngine;

public class Flee : BaseRecyclableState
{
    RecyclableNPC npc;
    Vector3 direction;

    public Flee(RecyclableNPC npc) : base(npc, npc)
    {
        this.npc = npc;
    }


    public override void Enter()
    {
        base.Enter();
        navigation.ClearDestination();
    }

    public override void Exit()
    {
        base.Exit();
        navigation.ClearDestination();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, npc.sightRange);


        direction = Vector3.zero;
        foreach (var item in colliders)
        {
            var contaminant = item.GetComponent<ContaminantNPC>();
            if (contaminant == null) continue;

            Vector3 from_contaminant = transform.position - contaminant.transform.position;

            direction += from_contaminant.normalized * (1 / from_contaminant.sqrMagnitude); // the further away the contaminant is, the less it will affect the direction
        }
        if (direction == Vector3.zero)
        {
            npc.SwitchState(npc.state_Idle);
        }
        else
        {
            direction.Normalize();
        }
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        navigation.SetDestination(transform.position + direction * npc.sightRange);
        
    }
}