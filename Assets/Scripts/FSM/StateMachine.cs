using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    State currentState;

    /// <summary>
    /// This event is invoked when the state is changed.
    /// </summary>
    public event OnStateChanged StateChanged;

    /// <summary>
    /// Method to switch states
    /// </summary>
    /// <param name="newState"></param>
    public void SwitchState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    /// <summary>
    /// Event callback
    /// </summary>
    /// <param name="prev">The previous state</param>
    /// <param name="next">The next state</param>
    public delegate void OnStateChanged(State prev, State next);

    
    /// <summary>
    /// Current active coroutine for the current state.
    /// </summary>
    public Coroutine currentStateCoroutine { get; private set; }


    /// <summary>
    /// Sets the initial state of the FSM. Subsequent calls are ignored!
    /// </summary>
    /// <param name="initialState">The initial state</param>
    public void SetInitialState(State initialState)
    {
        if (initialState == null) throw new NullReferenceException("The initial state cannot be null!");
        // Only allow this method to set the initial state once.
        if (currentState == null)
        {
            Transition(initialState);
        }
        else{
            Debug.LogWarning("Initial state already set! Calls to the method are ignored after the initial call!");
        }
    }

    /// <summary>
    /// Transitions to another state. Note that calling this will immediately stop the current state
    /// </summary>
    /// <param name="nextState">The next state</param>
    /// <exception cref="NullReferenceException">Do not attempt to set state to null!</exception>
    public void Transition(State nextState)
    {
        if (nextState == null) throw new NullReferenceException("Attempted to transition to null state!");

        var prev = currentState;
        // Immediately stop current state coroutine (To break out of it)
        if (currentStateCoroutine !=null) StopCoroutine(currentStateCoroutine);
        // Null coalescing operator still needed because initial state is null.
        currentState?.Exit();
        currentState = nextState;
        currentState.Enter();
        // Invoke event for side-effects.
        StateChanged?.Invoke(prev, nextState);
        // Start coroutine for current state
        currentStateCoroutine = StartCoroutine(currentState.Start());
    }
}
