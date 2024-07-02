using UnityEngine;
using Level.Bins;
using NPC.Contaminants.States;

namespace NPC.Contaminant
{
    public class ContaminantNPC : FSMRecyclableNPC, ILevelEntity {
        #region States
        public Idle state_Idle { get; private set; }
        public Chase state_Chase { get; private set; }

        #endregion

        public float move_speed;
        public float sightRange;
        public override RecyclableType recyclableType => RecyclableType.OTHERS;

        public void OnZoneStart()
        {
            // activate the contaminant
        }
    }    
}
