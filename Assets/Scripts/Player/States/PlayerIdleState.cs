using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : State<PlayerController>
{
    Vector2 move_input = Vector2.zero;

    public PlayerIdleState(StateMachine<PlayerController> fsm, PlayerController character) : base(fsm, character)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // play idle animation
    }

    public override void HandleInputs()
    {
        base.HandleInputs();
        // set movement input
        move_input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // check transition to move state
        if (move_input == Vector2.zero) return;
        fsm.SwitchState(character.MoveState);
    }
}
