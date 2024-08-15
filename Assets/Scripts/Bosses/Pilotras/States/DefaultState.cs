using System.Linq;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class DefaultState : State<PilotrasController>
    {
        public DefaultState(StateMachine<PilotrasController> fsm, PilotrasController character) : base(fsm, character)
        {
        }

        public override void Enter()
        {
            base.Enter();
            character.anim?.Play("Idle");
        }

        public override void HandleInputs()
        {
            base.HandleInputs();
            character.UpdateNPCCount();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // only enter states that spawn more NPCs if NPC count is below the max count
            if (character.data.npcCount.Values.Sum(x => x) < character.data.max_npc_count)
            {
                if (character.currentPhase > 1 && character.LaneAttackState.CanEnter)
                {
                    fsm.SwitchState(character.LaneAttackState);
                    return;
                }

                if (character.PlacingState.CanEnter)
                {
                    fsm.SwitchState(character.PlacingState);
                    return;
                }
            }

            if (character.BinDropState.CanEnter) 
                fsm.SwitchState(character.BinDropState);
        }
    }
}
