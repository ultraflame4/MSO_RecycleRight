using System.Collections;
using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class PlacingState : CooldownState<PilotrasController>
    {
        int amountToPlace;
        Coroutine coroutine_placing;

        public PlacingState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.placing_duration, character.behaviourData.placing_cooldown)
        {
            CanEnter = true;
        }

        public override void Enter()
        {
            duration = character.behaviourData.placing_duration;
            cooldown = character.behaviourData.placing_cooldown;
            base.Enter();
            amountToPlace = character.behaviourData.place_npc_amount;
            coroutine_placing = character.StartCoroutine(PlaceNPC());
        }

        public override void Exit()
        {
            base.Exit();

            if (coroutine_placing != null) 
            {
                character.StopCoroutine(coroutine_placing);
                coroutine_placing = null;
            }
        }

        /// <summary>
        /// Restart state cooldown
        /// </summary>
        public new void StartCooldown()
        {
            CanEnter = false;
            base.StartCooldown();
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
