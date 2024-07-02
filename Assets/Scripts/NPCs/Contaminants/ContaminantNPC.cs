using UnityEngine;
using Level.Bins;

namespace NPC.Contaminant
{
    public class ContaminantNPC : FSMRecyclableNPC, ILevelEntity {
        public float move_speed;
        public override RecyclableType recyclableType => RecyclableType.OTHERS;

        public void OnZoneStart()
        {
            // activate the contaminant
        }
    }    
}
