using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefaultState : State<PlayerController>
{
    Rigidbody2D rb;
    Vector2 move_input = Vector2.zero;

    public PlayerDefaultState(StateMachine<PlayerController> fsm, PlayerController character) : base(fsm, character)
    {
        // get reference to rigidbody component
        rb = character.GetComponent<Rigidbody2D>();
    }

    public override void Enter()
    {
        base.Enter();
        // play move animation
    }

    public override void HandleInputs()
    {
        base.HandleInputs();
        // set movement input
        move_input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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
        rb.velocity = move_input.normalized * character.Data.movementSpeed * Time.deltaTime;
    }
}
