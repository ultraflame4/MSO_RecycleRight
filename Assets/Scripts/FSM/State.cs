using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected StateMachine fsm;
    Coroutine coroutine;
    bool makeCoroutine;

    public State(StateMachine fsm, bool makeCoroutine = false)
    {
        this.fsm = fsm;
        this.makeCoroutine = makeCoroutine;
    }

    public virtual void Enter() 
    {
        coroutine = fsm.StartCoroutine();
    }

    public virtual void LogicUpdate() {}
    public virtual void PhysicsUpdate() {}

    public virtual void Exit() 
    {
        if (coroutine == null) return;
        fsm.StopCoroutine(coroutine);
    }

    IEnumerator WaitForState(float duration)
    {
        yield return new WaitForSeconds(duration);
    }
}
