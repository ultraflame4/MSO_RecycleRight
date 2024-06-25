using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : StateMachine<PlayerController>
{
    #region Inspector Fields
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] bool showGizmos = true;
    #endregion

    #region States
    public PlayerDefaultState DefaultState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    #endregion

    #region Other Properties
    public PlayerCharacter Data { get; private set; }
    public Transform pointer => transform.GetChild(0);
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
        DefaultState = new PlayerDefaultState(this, this);
        AttackState = new PlayerAttackState(this, this);
        // initialize state machine
        Initialize(DefaultState);
    }
    #endregion

    #region Event Listeners
    void OnCharacterChange(PlayerCharacter data)
    {
        Data = data;
    }
    #endregion

    #region Gizmos
    void OnDrawGizmosSelected() 
    {
        Gizmos.DrawWireSphere(pointer.position, attackRange);
    }
    #endregion
}
