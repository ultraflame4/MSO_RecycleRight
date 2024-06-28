using UnityEngine;

public class StateMachine<T> : MonoBehaviour
{
    protected State<T> currentState;

    /// <summary>
    /// Event callback
    /// </summary>
    /// <param name="prev">The previous state</param>
    /// <param name="next">The next state</param>
    public delegate void OnStateChanged(State<T> prev, State<T> next);

    /// <summary>
    /// This event is invoked when the state is changed.
    /// </summary>
    public event OnStateChanged StateChanged;

    /// <summary>
    /// Method to initialize the state machine and enter the starting state. 
    /// </summary>
    /// <param name="state">State to start in</param>
    public void Initialize(State<T> state)
    {
        currentState = state;
        currentState?.Enter();
    }

    /// <summary>
    /// Method to switch states. 
    /// </summary>
    /// <param name="nextState">State to transition into</param>
    public void SwitchState(State<T> nextState)
    {
        // set previous state
        State<T> prev = nextState;
        // change state
        currentState?.Exit();
        currentState = nextState;
        currentState?.Enter();
        // Invoke event for side-effects.
        StateChanged?.Invoke(prev, nextState);
    }

    #region Monobehaviour Callbacks

    void Update() 
    {
        currentState?.HandleInputs();
        currentState?.LogicUpdate();
    }

    void FixedUpdate() 
    {
        currentState?.PhysicsUpdate();
    }

    public virtual void OnDrawGizmosSelected() {
        currentState?.OnDrawGizmosSelected();
    }
    public virtual void OnDrawGizmos() {
        currentState?.OnDrawGizmos();
    }
    #endregion
}
