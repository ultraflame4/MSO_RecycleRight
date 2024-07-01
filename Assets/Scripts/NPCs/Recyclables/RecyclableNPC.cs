using UnityEngine;

public class RecyclableNPC : FSMRecyclableNPC, INPCBody
{

    #region States
    public RecyclableIdle state_Idle { get; private set; }
    public Flee state_Flee { get; private set; }
    public Stunned state_Stunned { get; private set; }
    #endregion
    #region References
    [field: SerializeField]
    public virtual LevelManager levelManager { get; private set; }
    #endregion

    #region Config
    [SerializeField]
    private RecyclableType _recyclableType;
    public override RecyclableType recyclableType => _recyclableType;
    public float sightRange = 3f;
    #endregion



    public ContaminantNPC nearestContaminant { get; private set; } = null;

    private void Start()
    {
        state_Idle = new(this);
        state_Idle.levelManager = levelManager;
        state_Stunned = new(this);
        state_Flee = new(this);

        Initialize(state_Idle);
    }
    public void Hit(float damage, float stun_duration)
    {
        Debug.Log("Hit");
        state_Stunned.stun_timer = stun_duration;
        SwitchState(state_Stunned);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}