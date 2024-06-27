using UnityEngine;

public class RecyclableNPC : FSMRecyclableNPC
{

    #region States
    private RandomWalk state_RandWalk;
    #endregion

    [field: SerializeField]
    public virtual RecyclableType recyclableType { get; private set; }
    public override bool is_contaminated => false;

    private void Start()
    {
        state_RandWalk = new (this, this);

        Initialize(state_RandWalk);
    }

}