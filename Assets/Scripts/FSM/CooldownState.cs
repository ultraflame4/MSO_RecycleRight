namespace Patterns.FSM
{
    public class CooldownState<T> : CoroutineState<T>
    {
        protected float cooldown;
        private bool earlyExit = false;
        public bool CanEnter { get; protected set; } = false;

        /// <summary>
        /// Constructor to create a state that can only be entered after a cooldown after exiting
        /// </summary>
        /// <param name="fsm">Reference to state machine of type StateMachine</param>
        /// <param name="character">Reference to character of generic type</param>
        /// <param name="nextState">State to transition to after set duration</param>
        /// <param name="duration">Duration to wait before transitioning to next state</param>
        /// <param name="cooldown">Cooldown before being able to enter state again</param>
        public CooldownState(StateMachine<T> fsm, T character, State<T> nextState, float duration, float cooldown) 
            : base(fsm, character, nextState, duration)
        {
            this.cooldown = cooldown;
            if (!CanEnter) StartCooldown();
        }

        public override void Enter()
        {
            if (!CanEnter)
            {
                earlyExit = true;
                fsm.SwitchState(nextState);
                return;
            }

            CanEnter = false;
            base.Enter();
        }

        public override void Exit()
        {
            if (earlyExit)
            {
                earlyExit = false;
                return;
            }
            
            base.Exit();
            StartCooldown();
        }

        protected virtual void StartCooldown()
        {
            fsm.StartCoroutine(WaitForSeconds(cooldown, () => CanEnter = true));
        }
    }
}
