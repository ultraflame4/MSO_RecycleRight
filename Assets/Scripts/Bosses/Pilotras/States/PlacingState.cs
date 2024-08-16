using System.Collections;
using UnityEngine;
using Patterns.FSM;

namespace Bosses.Pilotras.FSM
{
    public class PlacingState : CooldownState<PilotrasController>
    {
        int amountToPlace;
        bool throwAlternative;
        Coroutine coroutine_placing;

        public PlacingState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.placing_duration, character.behaviourData.placing_cooldown)
        {
            CanEnter = true;
        }

        public override void Enter()
        {
            // check for chance to enter meteor attack state
            if (CanEnter && Random.Range(0f, 1f) <= character.meteorAttackChance)
            {
                fsm.SwitchState(character.MeteorShowerAttackState);
                return;
            }

            // handle normal placing state
            duration = character.behaviourData.placing_duration;
            cooldown = character.behaviourData.placing_cooldown;
            base.Enter();
            throwAlternative = false;
            amountToPlace = character.behaviourData.place_npc_amount;
            coroutine_placing = character.StartCoroutine(PlaceNPC());
        }

        public override void Exit()
        {
            CanEnter = false;
            base.Exit();
            character.anim?.SetBool("ThrowAlternative", false);
            character.anim?.ResetTrigger("Throw");
            if (coroutine_placing == null) return;
            character.StopCoroutine(coroutine_placing);
            coroutine_placing = null;
        }

        IEnumerator PlaceNPC()
        {
            GameObject obj = character.PlaceNPC(character.transform.position);
            character.anim?.SetBool("ThrowAlternative", throwAlternative);
            character.anim?.SetTrigger("Throw");
            throwAlternative = !throwAlternative;
            obj?.SetActive(false);
            character.StartCoroutine(character.Throw(character.behaviourData.placing_speed, character.behaviourData.placing_delay, 
                obj, character.GetRandomPositionInZone()));
            yield return new WaitForSeconds(duration / amountToPlace);
            coroutine_placing = character.StartCoroutine(PlaceNPC());
        }
    }
}
