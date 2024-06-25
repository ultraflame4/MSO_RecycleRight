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

    #region Other Properties
    public PlayerCharacter Data { get; private set; }
    #endregion

    #region Components
    CharacterManager characterManager;
    #endregion

    #region Monobehaviour Callbacks
    void Awake()
    {
        // get componenets
        characterManager = GetComponent<CharacterManager>();
        // subscribe to character change event
        characterManager.CharacterChanged += OnCharacterChange;
    }

    void Start()
    {
        // initialize states
        IdleState = new PlayerIdleState(this, this);
        MoveState = new PlayerMoveState(this, this);
        AttackState = new PlayerAttackState(this, this);
        // initialize state machine
        Initialize(IdleState);
    }
    #endregion

    #region Event Listeners
    void OnCharacterChange(PlayerCharacter data)
    {
        Data = data;
    }
    #endregion
}
