using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : StateMachine<PlayerController>
{
    #region Inspector Fields
    [Header("References")]
    [SerializeField] private LevelManager levelManager;
    #endregion

    #region States
    public PlayerDefaultState DefaultState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerMoveToZoneState MoveToZoneState { get; private set; }
    #endregion

    #region Other Properties
    public PlayerCharacter Data { get; private set; }
    public Behaviour CharacterBehaviour { get; private set; }
    public Transform pointer => transform.GetChild(0);
    #endregion

    #region Other Private Components
    CharacterManager characterManager;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        // get componenets
        characterManager = GetComponent<CharacterManager>();
        // set character to first character instance
        OnCharacterChange(characterManager.character_instances[0]);
        // subscribe to character change event
        characterManager.CharacterChanged += OnCharacterChange;

        // initialize states
        DefaultState = new PlayerDefaultState(this, this);
        AttackState = new PlayerAttackState(this, this);
        MoveToZoneState = new PlayerMoveToZoneState(this, this);
        // initialize state machine
        Initialize(DefaultState);

        // subscribe to zone change event if level manager is not null
        if (levelManager != null)
            levelManager.ZoneChanged += MoveToZoneState.OnZoneChange;
    }
    #endregion

    #region Event Listeners
    void OnCharacterChange(PlayerCharacter data)
    {
        // set data to new character
        Data = data;
        // set behaviour to new character behaviour
        Behaviour newBehaviour = data.GetComponent<Behaviour>();
        // ensure behaviour is not null before assigning to variable
        if (newBehaviour == null) return;
        CharacterBehaviour = newBehaviour;
    }
    #endregion
}
