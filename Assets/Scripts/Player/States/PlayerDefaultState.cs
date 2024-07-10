using UnityEngine;
using Patterns.FSM;

namespace Player.FSM
{
    public class PlayerDefaultState : State<PlayerController>
    {
        Rigidbody2D rb;
        Vector2 move_input = Vector2.zero;
        bool attack_input, skill_input = false;

        public PlayerDefaultState(StateMachine<PlayerController> fsm, PlayerController character) : base(fsm, character)
        {
            // get reference to rigidbody component
            rb = character.GetComponent<Rigidbody2D>();
        }

        public override void Enter()
        {
            base.Enter();
            // allow character switching
            character.CharacterManager.CanSwitchCharacters = true;
            // play idle animation
            character.anim?.Play("Idle");
            // reset inputs
            move_input = Vector2.zero;
            attack_input = false;
            skill_input = false;
        }

        public override void HandleInputs()
        {
            base.HandleInputs();
            // update pointer direction
            character.PointerManager.UpdatePointer();
            // set movement input
            move_input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            // update animation
            character.anim?.SetBool("IsMoving", 
                move_input != Vector2.zero && character.Data.movementSpeed > 0f);
            // set attack input
            attack_input = Input.GetMouseButtonDown(0);
            // set skill input
            skill_input = Input.GetKeyDown(KeyCode.E);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // update sprite flip if moving, and movement speed is not 0
            if (move_input.x != 0 && character.Data.movementSpeed > 0f) 
                character.Data.renderer.flipX = move_input.x < 0f;

            // check for transition to skill state, ensure can trigger skill
            if (skill_input && character.CharacterBehaviour.CanTriggerSkill)
            {
                fsm.SwitchState(character.SkillState);
                return;
            }
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
            rb.velocity = move_input.normalized * character.Data.movementSpeed * 
                character.Data.movementMultiplier * Time.deltaTime;
        }

        public override void Exit()
        {
            base.Exit();
            // set is moving animation bool to false
            character.anim?.SetBool("IsMoving", false);
            // set sprite flip to pointer direction when exiting default state
            character.Data.renderer.flipX = character.pointer.up.x < 0f;
            // disallow character switching when not in default state
            character.CharacterManager.CanSwitchCharacters = false;
            // reset velocity when exiting state
            rb.velocity = Vector2.zero;
        }
    }
}
