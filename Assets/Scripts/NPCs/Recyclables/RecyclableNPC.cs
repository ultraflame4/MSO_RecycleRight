using UnityEngine;

public class RecyclableNPC : FSMRecyclableNPC, INPCBody
{

    #region States
    private RandomWalk state_RandWalk;
    private Stunned state_Stunned;
    #endregion

    [SerializeField]
    private RecyclableType _recyclableType;
    
    public override RecyclableType recyclableType => _recyclableType;

    [field: SerializeField]
    public virtual LevelManager levelManager { get; private set; }


    private void Start()
    {
        state_RandWalk = new(this, this);
        state_RandWalk.levelManager = levelManager;
        state_Stunned = new(this, this);

        Initialize(state_RandWalk);
    }
    public void Hit(float damage)
    {
        Debug.Log("Hit");
        SwitchState(state_Stunned);
    }

}