using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System.Reflection;
using UnityEngine.VFX;
using System.Linq;
using UnityEngine;
using System;

public class Bob : MonoBehaviour
{
    private readonly Dictionary<Type, BobState> states = new();

    private WaitWhile waitForStateExecution;
    private Coroutine attackRoutine;
    private BobState currentState;

    [Header("Bomb state settings")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform bombParentTransform;

    [Header("IceBeam state settings")]
    [SerializeField] private VisualEffect iceChargeup;
    [SerializeField] private VisualEffect iceBeam;
    [SerializeField] private GameObject hitEffect;

    [Header("Tail state settings")]
    [SerializeField] private float tailAttackTime;
    [SerializeField] private int tailProjectileCount;
    [SerializeField] private float tailAttackArc;
    [SerializeField] private GameObject tailProjectile;
    [SerializeField] private float tailProjectileStrength;

    [Header("Stomp state settings")]
    [SerializeField] private int minBrittleRequirement;

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

        //Initialize the states with their needed references and values
        InitializeStates();

        //Start the attack routine
        StartAttack();
    }

    private void InitializeStates()
    {
        foreach (var state in states.Values)
        {
            switch (state)
            {
                case BobTailState tailState:
                    tailState.Initialize(tailAttackTime, tailProjectileCount, tailAttackArc, tailProjectile, transform, tailProjectileStrength);
                    break;
                case BobIceState iceState: 
                    iceState.Initialize(transform, iceChargeup, iceBeam, hitEffect); 
                    break;
                case BobBombState bombState:
                    bombState.Initialize(bombPrefab, bombParentTransform);
                    break;
                case BobIdleState idleState:
                    idleState.Initialize(transform);
                    break;
            }
        }
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
        LoadState<BobIceState>();
        yield return waitForStateExecution;

        //Idle after attack state
        LoadState<BobIdleState>(2f);
        yield return waitForStateExecution;

        //Tail attack
        LoadState<BobTailState>();
        yield return waitForStateExecution;

        //Idle after attack state
        LoadState<BobIdleState>(4f);
        yield return waitForStateExecution;

        //Choose the state based on whether there are any brittle ice platforms
        if (IcePlatformManager.Instance.BrittlePlatformCount >= minBrittleRequirement)
        {
            //Stomp attack
            LoadState<BobStompState>();
            yield return waitForStateExecution;
        }
        else
        {
            //Bomb attack
            LoadState<BobBombState>();
            yield return waitForStateExecution;
        }

        //Idle after attack state
        LoadState<BobIdleState>(3f);
        yield return waitForStateExecution;

        //Restart the coroutine
        callback.Invoke();
    }
}
