using UnityEngine;

public class ContaminantNPC : FSMRecyclableNPC {
    public float move_speed;
    public override bool is_contaminated => true;

}