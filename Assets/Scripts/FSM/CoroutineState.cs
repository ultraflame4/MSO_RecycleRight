using System;
using System.Collections;
using UnityEngine;

public class CoroutineState<T> : State<T>
{
    private Coroutine coroutine;
    protected State<T> nextState;
    protected float duration;

    public CoroutineState(StateMachine<T> fsm, T character, State<T> nextState, float duration) : base(fsm, character)
    {
        this.fsm = fsm;
        this.character = character;
        this.nextState = nextState;
        this.duration = duration;
    }

    public override void Enter()
    {
        base.Enter();
        // start coroutine to count state duration
        coroutine = fsm.StartCoroutine(WaitForSeconds(duration, () => 
            {
                coroutine = null;
                fsm.SwitchState(nextState);
            }
        ));
    }

    public override void Exit() 
    {
        base.Exit();
        // stop coroutine when changing state
        if (coroutine == null) return;
        fsm.StopCoroutine(coroutine);
    }

    /// <summary>
    /// Coroutine to wait for a set duration in a state
    /// </summary>
    /// <param name="duration">Duration to wait</param>
    /// <param name="callback">Method to call after the wait duration</param>
    /// <returns></returns>
    protected IEnumerator WaitForSeconds(float duration, Action callback = null)
    {
        yield return new WaitForSeconds(duration);
        callback?.Invoke();
    }
}
