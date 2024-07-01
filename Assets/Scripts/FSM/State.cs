public class State<T>
{
    protected StateMachine<T> fsm;
    protected T character;

    /// <summary>
    /// Constructor to create a state
    /// </summary>
    /// <param name="fsm">Reference to state machine of type StateMachine</param>
    /// <param name="character">Reference to character of generic type</param>
    public State(StateMachine<T> fsm, T character)
    {
        this.fsm = fsm;
        this.character = character;
    }

    public virtual void Enter() {}
    public virtual void HandleInputs() {}
    public virtual void LogicUpdate() {}
    public virtual void PhysicsUpdate() {}
    public virtual void Exit() {}

    public virtual void OnDrawGizmosSelected() {}
    public virtual void OnDrawGizmos() {}
}
