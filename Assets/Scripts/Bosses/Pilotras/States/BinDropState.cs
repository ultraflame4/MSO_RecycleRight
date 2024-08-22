using UnityEngine;
using Patterns.FSM;
using Interfaces;
using Bosses.Pilotras.Bin;
using Level.Bins;
using NPC;

namespace Bosses.Pilotras.FSM
{
    public class BinDropState : CooldownState<PilotrasController>
    {
        public PilotrasBinManager binManager => character.binManager;
        public PilotrasBinCoroutineManager coroutineManager => character.binCoroutineManager;
        public Vector2 binContainerLocation, shockwaveCenter, shockwaveSize;
        public GameObject indicator;
        public float xPos, yPos;
        public float _duration => duration;

        public BinDropState(StateMachine<PilotrasController> fsm, PilotrasController character) : 
            base(fsm, character, character.PostBinDropStunState, 0f, character.behaviourData.bin_drop_cooldown)
        {
        }

        public override void Enter()
        {
            // set duration and cooldown of state
            duration = character.behaviourData.bin_drop_duration + 
                character.behaviourData.bin_drop_speed + 
                character.data.attack_delay;
            cooldown = character.behaviourData.bin_drop_cooldown;

            base.Enter();
            
            // update current number of NPCs and reset selected bins
            character.UpdateNPCCount();
            binManager.selectedBins.Clear();
            // reset bin points in this drop instance
            binManager.scoredInInstance = 0f;

            // load bins to be dropped
            binManager.LoadBins();
            // load second bin if phase is more than one
            if (character.currentPhase > 1) binManager.LoadBins();

            // start coroutine to count delay before dropping bin
            coroutineManager.StartCoroutine(coroutineManager.DelayedBinDrop());
            // start coroutine to await end of state
            coroutineManager.StartCoroutine(coroutineManager.AwaitRaiseBin());
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (binManager.scoredInInstance < character.behaviourData.topple_threshold) return;
            fsm.SwitchState(character.ToppleState);
        }

        public override void Exit()
        {
            base.Exit();
            // reset coroutines
            coroutineManager.StopAllCoroutines();

            // reset all selected bins
            foreach (RecyclingBin bin in binManager.selectedBins)
            {
                binManager.ResetBin(bin);
            }

            // reset indicator
            if (indicator == null) return;
            indicator.SetActive(false);
            indicator = null;
        }

        /// <summary>
        /// Method to draw debug lines to show shockwave range
        /// </summary>
        public void DrawDebug()
        {
            if (shockwaveCenter == Vector2.zero || shockwaveSize == Vector2.zero) return;
            Vector2 topLeft = new Vector2(shockwaveCenter.x - shockwaveSize.x, shockwaveCenter.y + shockwaveSize.y);
            Vector2 topRight = new Vector2(shockwaveCenter.x + shockwaveSize.x, shockwaveCenter.y + shockwaveSize.y);
            Vector2 bottomLeft = new Vector2(shockwaveCenter.x - shockwaveSize.x, shockwaveCenter.y - shockwaveSize.y);
            Vector2 bottomRight = new Vector2(shockwaveCenter.x + shockwaveSize.x, shockwaveCenter.y - shockwaveSize.y);
            Debug.DrawLine(topLeft, topRight, Color.magenta);
            Debug.DrawLine(topRight, bottomRight, Color.magenta);
            Debug.DrawLine(bottomRight, bottomLeft, Color.magenta);
            Debug.DrawLine(bottomLeft, topLeft, Color.magenta);
        }

        public void SendShockwave(Vector2 center, Vector2 detectionSize)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(center, detectionSize, 
                character.behaviourData.drop_detection_mask);
            Rigidbody2D hitRB;

            // apply knockback to hit NPCs
            foreach (Collider2D hit in hits)
            {
                if (hit.transform == character.transform) continue;
                hitRB = hit.GetComponent<Rigidbody2D>();
                if (hitRB == null) continue;
                hitRB.AddForce((hit.transform.position - character.transform.position).normalized * 
                    character.behaviourData.bin_drop_force, ForceMode2D.Impulse);
            }

            hits = Physics2D.OverlapBoxAll(center, detectionSize, 
                character.data.hit_mask);
            
            // apply knockback and damage to player
            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent<IDamagable>(out IDamagable damagable))
                    damagable.Damage(character.data.bin_drop_damage);

                if (hit.TryGetComponent<IStunnable>(out IStunnable stunnable))
                    stunnable.Stun(character.data.bin_drop_player_stun_duration);

                hitRB = hit.GetComponentInParent<Rigidbody2D>();
                if (hitRB == null) continue;
                hitRB.AddForce((hit.transform.position - character.transform.position).normalized * 
                    character.behaviourData.bin_drop_force, ForceMode2D.Impulse);
            }
        }

        public void StunNPCs()
        {
            foreach (FSMRecyclableNPC recyclable in character.data.recyclables)
            {
                if (recyclable == null) continue;
                if (!recyclable.TryGetComponent<IStunnable>(out IStunnable stunnable)) continue;
                stunnable.Stun(duration);
            }
        }
    }
}
