using UnityEngine;

public class RecyclableNPC : FSMRecyclableNPC
{

    #region States
    private RandomWalk state_RandWalk;
    #endregion

    [ SerializeField]
    private RecyclableType _recyclableType;
    public override RecyclableType recyclableType => _recyclableType;

     [field: SerializeField]
    public virtual LevelManager levelManager { get; private set; }
    public override bool is_contaminant => false;

    private void Start()
    {
        state_RandWalk = new (this, this);
        state_RandWalk.levelManager = levelManager;

        Initialize(state_RandWalk);
    }

}