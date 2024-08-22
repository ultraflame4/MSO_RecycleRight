using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class ToppleState : CoroutineState<PilotrasController>
    {
        Collider2D collider;
        Vector2 originalColliderOffset;
        float originalDamageScale = 1f;

        public ToppleState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.topple_duration)
        {
            originalDamageScale = character.data.damageTakenScale;
            collider = character.GetComponent<Collider2D>();
            if (collider == null) return;
            originalColliderOffset = collider.offset;
        }

        public override void Enter()
        {
            base.Enter();
            character.anim?.Play("Topple Over");
            // increase damage scale to take more damage when in this state
            character.data.damageTakenScale = character.behaviourData.topple_damage_multiplier;
            // offset collider
            collider.offset = character.behaviourData.collider_offset;
        }

        public override void Exit()
        {
            base.Exit();
            character.anim?.Play("Topple Over (Reverse)");
            // revert damage scale back to original
            character.data.damageTakenScale = originalDamageScale;
            // revert colldier offset to original offset
            collider.offset = originalColliderOffset;
        }
    }
}
