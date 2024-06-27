using UnityEngine;

public class RecyclableNPC : BaseRecyclableNPC
{
    public float move_speed;
    [field: SerializeField]
    public virtual RecyclableType recyclableType { get; private set; }
    public override bool is_contaminated => false;

}