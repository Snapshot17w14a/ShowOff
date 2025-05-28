using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using static BobAttackPattern;

public class Bob : MonoBehaviour
{
    private readonly Dictionary<Type, BobState> states = new();

    private WaitWhile waitForStateExecution;
    private Coroutine attackRoutine;
    private BobState currentState;

    [Header("Attack Pattern")]
    [SerializeField] private BobAttackPattern attackPattern;
    [SerializeField] private bool loopPattern = true;

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
    [SerializeField] private Transform needleParentTransform;

    [Header("Stomp state settings")]
    [SerializeField] private int minBrittleRequirement;
    [SerializeField] private GameObject iciclePrefab;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float knokbackRange;
    [SerializeField] private float knockbackForce;
    [SerializeField] private VisualEffect stompEffect;

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
                    tailState.Initialize(tailAttackTime, tailProjectileCount, tailAttackArc, tailProjectile, transform, tailProjectileStrength, needleParentTransform);
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
                case BobStompState stompState:
                    stompState.Initialize(iciclePrefab, spawnRadius, knokbackRange, knockbackForce, stompEffect);
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

    private void LoadState(Type type, params object[] parameters)
    {
        currentState?.UnloadState();
        currentState = states[type];
        currentState.LoadState(parameters);
        onStateChange?.Invoke();
    }

    //Stop the previous coroutine, start a new coroutine and keep a reference to the handle
    public void StartAttack()
    {
        if (attackRoutine != null) StopAttack();
        //attackRoutine = StartCoroutine(AttackPattern(StartAttack));
        attackRoutine = StartCoroutine(TestPatternFetcher(StartAttack));
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

    private IEnumerator TestPatternFetcher(Action callback)
    {
        for (int i = 0; i < attackPattern.StateCount; i++)
        {
            var nextState = attackPattern.NextState();
            Type type = MapContainerToType(nextState);

            if (nextState.State == BobStates.Idle) LoadState(type, nextState.time);
            else if (nextState.State == BobStates.HeavyStomp || nextState.State == BobStates.SpruceBomb)
            {
                if (IcePlatformManager.Instance.BrittlePlatformCount >= minBrittleRequirement) LoadState<BobStompState>();
                else LoadState<BobBombState>();
            }
            else LoadState(type);

            yield return waitForStateExecution;
        }

        if (loopPattern) callback.Invoke();
    }

    private Type MapContainerToType(BobAttackContainer container) => container.State switch
    {
        BobStates.Idle => typeof(BobIdleState),
        BobStates.IceBeam => typeof(BobIceState),
        BobStates.TailBurst => typeof(BobTailState),
        BobStates.SpruceBomb => typeof(BobBombState),
        BobStates.HeavyStomp => typeof(BobStompState),
        //BobStates.Star
        //BobStates.BobsRage
        _ => null
    };
}
