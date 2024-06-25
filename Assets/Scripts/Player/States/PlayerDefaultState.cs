using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefaultState : State<PlayerController>
{
    Rigidbody2D rb;
    Vector2 move_input = Vector2.zero;
    bool attack_input = false;

    public PlayerDefaultState(StateMachine<PlayerController> fsm, PlayerController character) : base(fsm, character)
    {
        // get reference to rigidbody component
        rb = character.GetComponent<Rigidbody2D>();
    }

    public override void Enter()
    {
        base.Enter();
        // reset inputs
        move_input = Vector2.zero;
        attack_input = false;
    }

    public override void HandleInputs()
    {
        base.HandleInputs();
        // set movement input
        move_input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // set attack input
        attack_input = Input.GetMouseButtonDown(0);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // check for transition to attack state
        if (attack_input)
        {
            fsm.SwitchState(character.AttackState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // ensure rigidbody is not null
        if (rb == null)
        {
            Debug.LogError("Rigidbody reference is null. (PlayerMoveState.cs)");
            return;
        }

        // set velocity based on movement input
        rb.velocity = move_input * character.Data.movementSpeed * Time.deltaTime;
    }

    public override void Exit()
    {
        base.Exit();
        // reset velocity when exiting state
        rb.velocity = Vector2.zero;
    }
}
