using UnityEngine;

public class FSMRecyclableNPC : StateMachine<FSMRecyclableNPC>
{
    [field: SerializeField]
    public Navigation navigation {get; private set;}
    public virtual RecyclableType recyclableType { get; }
    public virtual bool is_food { get; }
    public virtual bool is_contaminated { get; }

}
