using UnityEngine;
using Level.Bins;
using NPC.Contaminants.States;
using Player;

namespace NPC.Contaminant
{
    public class ContaminantNPC : FSMRecyclableNPC, ILevelEntity
    {
        #region States
        public DetectTarget state_Idle { get; private set; }
        public ChaseRecyclable state_ChaseRecyclable { get; private set; }
        public ChasePlayer state_ChasePlayer { get; private set; }
        #endregion

        public float move_speed;
        public float sightRange = 3f;
        public override RecyclableType recyclableType => RecyclableType.OTHERS;
        public bool playerInSight => PlayerController.Instance != null && Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < sightRange;

        private void Start()
        {
            state_Idle = new DetectTarget(this);
            state_ChaseRecyclable = new ChaseRecyclable(this);
            state_ChasePlayer = new ChasePlayer(this);
            SwitchState(state_Idle);
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        public void OnZoneStart()
        {
            // activate the contaminant
        }
    }
}
