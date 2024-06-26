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
        coroutine = fsm.StartCoroutine(WaitForState());
    }

    public override void Exit() 
    {
        base.Exit();
        // stop coroutine when changing state
        if (coroutine == null) return;
        fsm.StopCoroutine(coroutine);
    }

    IEnumerator WaitForState()
    {
        yield return new WaitForSeconds(duration);
        coroutine = null;
        fsm.SwitchState(nextState);
    }
}
