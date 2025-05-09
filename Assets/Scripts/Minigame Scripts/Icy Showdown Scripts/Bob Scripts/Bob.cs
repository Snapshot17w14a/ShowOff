using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;

public class Bob : MonoBehaviour
{
    private readonly Dictionary<Type, BobState> states = new();

    private WaitWhile waitForStateExecution;
    private BobState currentState;
    private Coroutine attackRoutine;

    bool dummy = true;

    void Start()
    {
        var stateClasses = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(BobState)) && !t.IsAbstract && t.IsClass);

        foreach (var stateClass in stateClasses)
        {
            BobState createdState = (BobState)Activator.CreateInstance(stateClass);
            states.Add(stateClass, createdState);
        }

        LoadState(states[typeof(BobIdleState)]);

        waitForStateExecution = new(() => currentState.IsStateRunning);

        StartAttack();
    }

    private void LoadState(BobState state, params object[] parameters)
    {
        currentState?.UnloadState();
        currentState = state;
        currentState.LoadState(parameters);
    }

    public void StartAttack()
    {
        attackRoutine = StartCoroutine(AttackPattern(StartAttack));
    }

    public void StopAttack()
    {
        StopCoroutine(attackRoutine);
    }

    private void Update()
    {
        currentState?.TickState();
        dummy = !dummy;
    }

    private IEnumerator AttackPattern(Action callback)
    {
        //Ice attack
        LoadState(states[typeof(BobIceState)], transform);
        yield return waitForStateExecution;
        yield return new WaitForSeconds(2);

        //Tail attack
        LoadState(states[typeof(BobTailState)]);
        yield return waitForStateExecution;
        yield return new WaitForSeconds(4);

        if (/*TODO: check if brittle ice exists*/ true)
        {
            //Bomb attack
            LoadState(states[typeof(BobBombState)]);
            yield return waitForStateExecution;
        }
        else
        {
            //Stomp attack
            LoadState(states[typeof(BobStompState)]);
            yield return waitForStateExecution;
        }
        yield return new WaitForSeconds(3);

        callback.Invoke();
    }
}
