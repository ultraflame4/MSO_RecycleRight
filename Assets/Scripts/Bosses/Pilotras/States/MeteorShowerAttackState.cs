using System.Collections;
using UnityEngine;
using Patterns.FSM;
using Interfaces;

namespace Bosses.Pilotras.FSM
{
    public class MeteorShowerAttackState : CoroutineState<PilotrasController>
    {
        int amountToPlace;
        Coroutine coroutine_placing;

        public MeteorShowerAttackState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.DefaultState, character.behaviourData.meteor_attack_duration)
        {
        }

        public override void Enter()
        {
            duration = character.behaviourData.meteor_attack_duration;
            base.Enter();
            amountToPlace = character.behaviourData.meteor_attack_place_npc;
            coroutine_placing = character.StartCoroutine(PlaceNPC());
        }

        public override void Exit()
        {
            base.Exit();
            character.PlacingState?.Exit();

            if (coroutine_placing != null) 
            {
                character.StopCoroutine(coroutine_placing);
                coroutine_placing = null;
            }
        }

        IEnumerator PlaceNPC()
        {
            // pick a drop location and start dropping NPC
            Vector3 dropLocation = character.GetRandomPositionInZone();
            GameObject obj = character.PlaceNPC(new Vector3(dropLocation.x, character.yPosTop, character.transform.position.z));
            character.StartCoroutine(character.Throw(character.behaviourData.drop_speed, obj, dropLocation));
            // wait for NPC to land and handle landing
            character.StartCoroutine(WaitForLanding(dropLocation));
            // wait to throw next NPC
            yield return new WaitForSeconds(duration / amountToPlace);
            coroutine_placing = character.StartCoroutine(PlaceNPC());
        }

        IEnumerator WaitForLanding(Vector3 dropLocation)
        {
            yield return new WaitForSeconds(character.behaviourData.drop_speed);

            // get hit objects when landing
            Collider2D[] hits = Physics2D.OverlapCircleAll(dropLocation, character.behaviourData.drop_attack_range, character.data.hit_mask);

            foreach (Collider2D hit in hits)
            {
                hit.GetComponent<IDamagable>()?.Damage(character.data.meteor_attack_damage);
            }
        }
    }
}
