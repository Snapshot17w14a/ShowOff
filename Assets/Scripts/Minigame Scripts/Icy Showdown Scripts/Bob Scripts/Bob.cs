using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;

public class Bob : MonoBehaviour
{
    private readonly Dictionary<Type, BobState> states = new();

    private WaitWhile waitForStateExecution;
    private Coroutine attackRoutine;
    private BobState currentState;

    [Header("Events")]
    [SerializeField] private UnityEvent onStateChange;

    void Start()
    {
        //Get all classes that inherit from BobState and store their Types
        var stateClasses = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(BobState)) && !t.IsAbstract && t.IsClass);

        //Instantiate each state and store them in the dictionary with their Type being the key
        foreach (var stateClass in stateClasses) states.Add(stateClass, (BobState)Activator.CreateInstance(stateClass));

        //Set up the wait condition for the coroutine
        waitForStateExecution = new(() => currentState.IsStateRunning);

        //Start the attack routine
        StartAttack();
    }

    //Load the state and invoke the onStateChange event
    private void LoadState<T>(params object[] parameters) where T : BobState
    {
        currentState?.UnloadState();
        currentState = states[typeof(T)];
        currentState.LoadState(parameters);
        onStateChange?.Invoke();
    }

    //Stop the previous coroutine, start a new coroutine and keep a reference to the handle
    public void StartAttack()
    {
        if (attackRoutine != null) StopAttack();
        attackRoutine = StartCoroutine(AttackPattern(StartAttack));
    }

    //Stop the coroutine using the currently stored coroutine handle
    public void StopAttack() => StopCoroutine(attackRoutine);

    //Update the current state class
    private void Update() => currentState?.TickState();

    //Coroutine for the attack pattern of bob
    private IEnumerator AttackPattern(Action callback)
    {
        //Ice attack
        LoadState<BobIceState>(transform);
        yield return waitForStateExecution;

        //Idle after attack state
        LoadState<BobIdleState>(transform, 2f);
        yield return waitForStateExecution;

        //Tail attack
        LoadState<BobTailState>();
        yield return waitForStateExecution;

        //Idle after attack state
        LoadState<BobIdleState>(transform, 4f);
        yield return waitForStateExecution;

        //TODO: check if brittle ice exists
        //Chose the state based on whether there is any brittle ice
        if (true)
        {
            //Bomb attack
            LoadState<BobBombState>();
            yield return waitForStateExecution;
        }
        else
        {
            //Stomp attack
#pragma warning disable CS0162 // Unreachable code detected
            LoadState<BobStompState>();
#pragma warning restore CS0162 // Unreachable code detected
            yield return waitForStateExecution;
        }

        //Idle after attack state
        LoadState<BobIdleState>(transform, 3f);
        yield return waitForStateExecution;

        //Restart the coroutine
        callback.Invoke();
    }
}
