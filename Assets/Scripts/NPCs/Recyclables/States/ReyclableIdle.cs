using System.Linq;
using UnityEngine;

public class RecyclableIdle : RandomWalk
{
    RecyclableNPC npc;
    public RecyclableIdle(RecyclableNPC npc) : base(npc, npc)
    {
        this.npc = npc;
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, npc.sightRange);
        // todo in future remove any check and just check for enemy layer
        bool nearbyEnemy = colliders.Any(x=>x.GetComponent<ContaminantNPC>() != null);
        if (nearbyEnemy)
        {
            npc.SwitchState(npc.state_Flee);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
       
    }
}