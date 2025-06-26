using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using static BobAttackPattern;

public class Bob : MonoBehaviour
{
    private readonly Dictionary<Type, BobState> states = new();

    private WaitWhile waitForStateExecution;
    private Coroutine attackRoutine;
    private BobState currentState;

    [Header("Attack Pattern")]
    [SerializeField] private BobAttackPattern[] attackPatterns;
    private BobAttackPattern currentPattern;
    private int attackPatternIndex = -1;

    [Header("Beams layer mask")]
    [SerializeField] private LayerMask beamLayerMask;

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
    [SerializeField] private float spawnRadius;
    [SerializeField] private float knokbackRange;
    [SerializeField] private float knockbackForce;
    [SerializeField] private VisualEffect stompEffect;

    [Header("Rage state settings")]
    [SerializeField] private float crossDuration;
    [SerializeField] private float crossChargupTime;
    [SerializeField] private float rageStunTime;
    [SerializeField] private VisualEffect crossEffect;
    [SerializeField] private Volume globalVolume;

    [Header("Star state settings")]
    [SerializeField] private float timeBeforeSpawn;
    [SerializeField] private GameObject iciclePrefab;
    [SerializeField] private int minIcicleCount;
    [SerializeField] private int maxIcicleCount;
    [SerializeField] private VisualEffect startEffect;

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

        //Subscribe to clean up when the game is restarted
        EventBus<SceneRestart>.OnEvent += ResetAttackPatterns;

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
                    iceState.Initialize(transform, iceChargeup, iceBeam, hitEffect, beamLayerMask.value);
                    break;
                case BobBombState bombState:
                    bombState.Initialize(bombPrefab, bombParentTransform);
                    break;
                case BobIdleState idleState:
                    idleState.Initialize(transform);
                    break;
                case BobStompState stompState:
                    stompState.Initialize(knokbackRange, knockbackForce, stompEffect);
                    break;
                case BobRageState rageState:
                    rageState.Initialize(crossDuration, transform, crossEffect, crossChargupTime, rageStunTime, beamLayerMask.value, globalVolume);
                    break;
                case BobStarState starState:
                    starState.Initialize(timeBeforeSpawn, iciclePrefab, minIcicleCount, maxIcicleCount, startEffect);
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

        currentPattern = attackPatterns[++attackPatternIndex % attackPatterns.Length];

        attackRoutine = StartCoroutine(TestPatternFetcher(StartAttack));
    }

    //Stop the coroutine using the currently stored coroutine handle
    public void StopAttack() => StopCoroutine(attackRoutine);

    //Update the current state class
    private void Update() => currentState?.TickState();

    //Coroutine for attacking based on the provided attack pattern
    private IEnumerator TestPatternFetcher(Action callback)
    {
        for (int i = 0; i < currentPattern.StateCount; i++)
        {
            var nextState = currentPattern.NextState();
            Type type = MapEnumToType(nextState);

            if (nextState.State == BobStates.Idle) LoadState(type, nextState.time);
            else LoadState(type);

            yield return waitForStateExecution;
        }

        callback.Invoke();
    }

    private Type MapEnumToType(BobAttackContainer container) => container.State switch
    {
        BobStates.Idle => typeof(BobIdleState),
        BobStates.IceBeam => typeof(BobIceState),
        BobStates.TailBurst => typeof(BobTailState),
        BobStates.SpruceBomb => typeof(BobBombState),
        BobStates.HeavyStomp => typeof(BobStompState),
        BobStates.Star => typeof(BobStarState),
        BobStates.BobsRage => typeof(BobRageState),
        _ => null
    };

    private void ResetAttackPatterns(SceneRestart restart)
    {
        foreach (var pattern in attackPatterns) pattern.Reset();
    }

    private void OnDestroy()
    {
        EventBus<SceneRestart>.OnEvent -= ResetAttackPatterns;
    }
}
