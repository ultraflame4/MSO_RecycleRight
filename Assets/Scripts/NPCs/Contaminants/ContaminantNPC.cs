using UnityEngine;

public class ContaminantNPC : BaseRecyclableNPC {
    public float move_speed;
    public override bool is_contaminated => true;

}