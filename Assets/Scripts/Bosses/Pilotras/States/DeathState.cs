using System;
using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class DeathState : State<PilotrasController>
    {
        Collider2D collider;
        public event Action EndLevel;

        public DeathState(StateMachine<PilotrasController> fsm, PilotrasController character) : base(fsm, character)
        {
            collider = character.GetComponent<Collider2D>();
        }

        public override void Enter()
        {
            base.Enter();
            EndLevel?.Invoke();
            // set health bar
            character.HandlePhaseChange();
            // play topple over animation as death animation
            character.anim?.SetBool("Dead", true);
            // hide healthbar
            character.data.health_bar.gameObject.SetActive(false);
            // do not allow taking damage in this state
            character.data.damageTakenScale = 0f;
            // offset collider
            collider.offset = character.behaviourData.collider_offset;
        }
    }
}
