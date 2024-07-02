using UnityEngine;
using Level.Bins;
using NPC.Contaminants.States;
using Level;

namespace NPC.Contaminant
{
    public class ContaminantNPC : FSMRecyclableNPC, ILevelEntity
    {
        #region States
        public Idle state_Idle { get; private set; }
        public Chase state_Chase { get; private set; }
        #endregion

        public float move_speed;
        public float sightRange = 3f;
        public override RecyclableType recyclableType => RecyclableType.OTHERS;

        private void Start()
        {
            state_Idle = new Idle(this);
            state_Chase = new Chase(this);
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
