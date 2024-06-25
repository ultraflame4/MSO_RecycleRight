using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : StateMachine<PlayerController>
{
    #region States

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }

    #endregion

    #region Monobehaviour Callbacks

    void Awake()
    {
        // initialize states
        IdleState = new PlayerIdleState(this, this);
        MoveState = new PlayerMoveState(this, this);
        AttackState = new PlayerAttackState(this, this);
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialize state machine
        Initialize(IdleState);
    }

    #endregion
}
