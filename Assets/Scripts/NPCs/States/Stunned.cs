
using Patterns.FSM;
using UnityEngine;

namespace NPC.States
{
    public class Stunned : BaseRecyclableState
    {
        public float stun_timer;
        private BaseRecyclableState Idle;

        public Stunned(BaseRecyclableState idle, StateMachine<FSMRecyclableNPC> fsm, FSMRecyclableNPC character) : base(fsm, character)
        {
            Idle = idle;
        }

        bool original_navigation_enabled = true;
        public override void Enter()
        {
            base.Enter();
            if (navigation == null) return;

            original_navigation_enabled = navigation.enabled;
            if (navigation.enabled)
            {
                navigation.ClearDestination();
                navigation.enabled = false;
            }

        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            stun_timer -= Time.deltaTime;
            if (stun_timer <= 0)
            {
                stun_timer = 0;
                character.SwitchState(Idle);
            }
        }
        public override void Exit()
        {
            base.Exit();
            stun_timer = 0;
            if (navigation == null) return;
            navigation.enabled = original_navigation_enabled;
        }
    }
}
