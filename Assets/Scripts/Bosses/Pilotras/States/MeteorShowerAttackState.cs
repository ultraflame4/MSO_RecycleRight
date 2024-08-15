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
            base(fsm, character, character.DefaultState, character.behaviourData.meteor_attack_duration + character.data.attack_delay)
        {
        }

        public override void Enter()
        {
            duration = character.behaviourData.meteor_attack_duration + character.data.attack_delay;
            base.Enter();
            character.anim?.SetBool("SkillActive", true);
            character.UpdateNPCCount();
            amountToPlace = character.behaviourData.meteor_attack_place_npc;
            coroutine_placing = character.StartCoroutine(PlaceNPC());
        }

        public override void Exit()
        {
            base.Exit();
            character.PlacingState?.Exit();
            character.anim?.SetBool("SkillActive", false);
            if (coroutine_placing == null) return;
            character.StopCoroutine(coroutine_placing);
            coroutine_placing = null;
        }

        IEnumerator PlaceNPC()
        {
            // spawn NPC on a delay (wait for indicator)
            character.StartCoroutine(DelayedSpawn());
            // wait to throw next NPC
            yield return new WaitForSeconds(duration / amountToPlace);
            coroutine_placing = character.StartCoroutine(PlaceNPC());
        }

        IEnumerator DelayedSpawn()
        {
            // pick a random location to drop the NPC
            Vector3 dropLocation = character.GetRandomPositionInZone();
            // show indicator
            GameObject indicator = character.indicatorManager.Instantiate(0, dropLocation);
            // wait for attack delay
            yield return new WaitForSeconds(character.data.attack_delay);
            // hide indicator
            indicator?.SetActive(false);
            // start dropping NPC
            GameObject obj = character.PlaceNPC(new Vector3(dropLocation.x, character.yPosTop, character.transform.position.z));
            character.StartCoroutine(character.Throw(character.behaviourData.drop_speed, obj, dropLocation));
            // wait for NPC to land and handle landing
            yield return new WaitForSeconds(character.behaviourData.drop_speed);
            // get hit objects when landing
            Collider2D[] hits = Physics2D.OverlapCircleAll(dropLocation, character.behaviourData.drop_attack_range, character.data.hit_mask);
            foreach (Collider2D hit in hits)
            {
                if (!hit.TryGetComponent<IDamagable>(out IDamagable damagable)) continue;
                damagable.Damage(character.data.meteor_attack_damage);
            }
        }
    }
}
