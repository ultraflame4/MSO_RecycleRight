using UnityEngine;
using Level.Bins;

namespace NPC.Contaminant
{
    public class ContaminantNPC : FSMRecyclableNPC {
        public float move_speed;
        public override RecyclableType recyclableType => RecyclableType.OTHERS;

    }    
}
