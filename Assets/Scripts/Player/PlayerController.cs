using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : StateMachine<PlayerController>
{
    #region Inspector Fields
    [field: Header("Character Unique Behaviour Traits")]
    [field: SerializeField] public Behaviour CharacterAttack { get; private set; }
    [field: SerializeField] public Behaviour CharacterPassive { get; private set; }
    [field: SerializeField] public Behaviour CharacterSkill { get; private set; }
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
    void Start()
    {
        // get componenets
        characterManager = GetComponent<CharacterManager>();
        // set character data to first character instance
        Data = characterManager.character_instances[0];
        // subscribe to character change event
        characterManager.CharacterChanged += OnCharacterChange;

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
}
