public class State<T>
{
    protected StateMachine<T> fsm;
    protected T character;

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
}
