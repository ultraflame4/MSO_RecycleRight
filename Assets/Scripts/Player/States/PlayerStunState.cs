using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns.FSM;

namespace Player.FSM
{
    public class PlayerStunState : State<PlayerController>
    {
        public float duration = 0f;

        public PlayerStunState(StateMachine<PlayerController> fsm, PlayerController character) : base(fsm, character)
        {
        }

        public override void Enter()
        {
            base.Enter();
            // play stun animation
            character.anim?.Play("On Hit");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            duration -= Time.deltaTime;
            if (duration > 0f) return;
            fsm.SwitchState(character.DefaultState);
            duration = 0f;
        }
    }
}
