using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class PlacingState : CoroutineState<PilotrasController>
    {
        public bool CanEnter { get; private set; } = true;
        int amountToPlace;
        Coroutine coroutine_placing;

        public PlacingState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.placing_duration)
        {
        }

        public override void Enter()
        {
            if (!CanEnter)
            {
                fsm.SwitchState(character.DefaultState);
                return;
            }

            CanEnter = false;
            duration = character.behaviourData.placing_duration;
            amountToPlace = character.behaviourData.place_npc_amount;
            coroutine_placing = character.StartCoroutine(PlaceNPC());
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();

            if (coroutine_placing != null) 
            {
                character.StopCoroutine(coroutine_placing);
                coroutine_placing = null;
            }

            character.StartCoroutine(WaitForSeconds(character.behaviourData.placing_cooldown, () => CanEnter = true));
        }

        IEnumerator PlaceNPC()
        {
            GameObject obj = character.PlaceNPC(character.transform.position);
            if (obj != null) character.StartCoroutine(character.Throw(character.behaviourData.placing_speed, 
                    obj, character.GetRandomPositionInZone()));
            yield return new WaitForSeconds(duration / amountToPlace);
            coroutine_placing = character.StartCoroutine(PlaceNPC());
        }
    }
}
