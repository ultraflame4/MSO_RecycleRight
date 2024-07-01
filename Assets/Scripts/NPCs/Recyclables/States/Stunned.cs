
using UnityEngine;

public class Stunned : BaseRecyclableState
{
    public float stun_timer;
    private RecyclableNPC npc;
    public Stunned(RecyclableNPC npc) : base(npc, npc)
    {
        this.npc = npc;
    }

    public override void Enter()
    {
        base.Enter();
        navigation.ClearDestination();
        navigation.enabled = false;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        stun_timer -= Time.deltaTime;
        if (stun_timer <= 0)
        {
            character.SwitchState(npc.state_RandWalk);
        }
    }
    public override void Exit()
    {
        base.Exit();
        navigation.enabled = true;
    }
}