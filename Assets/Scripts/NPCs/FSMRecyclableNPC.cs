using UnityEngine;

public class FSMRecyclableNPC : StateMachine<FSMRecyclableNPC>
{
    [field: SerializeField]
    public Navigation navigation {get; private set;}
    /// <summary>
    /// The recyclable type of this NPC (if contamiant NPC, set to OTHERS)
    /// </summary>
    public virtual RecyclableType recyclableType { get; }
    /// <summary>
    /// Make this return true if this NPC is a food item. (Causes infestation)
    /// </summary>
    public virtual bool is_food { get; }

}
