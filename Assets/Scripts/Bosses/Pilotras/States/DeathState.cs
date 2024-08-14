using System;
using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class DeathState : State<PilotrasController>
    {
        public event Action EndLevel;

        public DeathState(StateMachine<PilotrasController> fsm, PilotrasController character) : base(fsm, character)
        {
        }

        public override void Enter()
        {
            base.Enter();
            EndLevel?.Invoke();
            // hide healthbar
            character.data.health_bar.gameObject.SetActive(false);
            // do not allow taking damage in this state
            character.data.damageTakenScale = 0f;
        }
    }
}
