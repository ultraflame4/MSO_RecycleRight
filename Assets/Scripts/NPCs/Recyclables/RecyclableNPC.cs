using UnityEngine;

public class RecyclableNPC : FSMRecyclableNPC, INPCBody
{

    #region States
    public RandomWalk state_RandWalk {get; private set;}
    public Stunned state_Stunned {get; private set;}
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
        state_Stunned = new(this);

        Initialize(state_RandWalk);
    }
    public void Hit(float damage, float stun_duration)
    {
        Debug.Log("Hit");
        state_Stunned.stun_timer = stun_duration;
        SwitchState(state_Stunned);
    }

}