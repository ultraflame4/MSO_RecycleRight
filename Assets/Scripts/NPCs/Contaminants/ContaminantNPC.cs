using UnityEngine;

public class ContaminantNPC : FSMRecyclableNPC {
    public float move_speed;
    public override RecyclableType recyclableType => RecyclableType.OTHERS;

}