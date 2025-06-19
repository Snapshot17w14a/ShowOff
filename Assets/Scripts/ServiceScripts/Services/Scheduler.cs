using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scheduler : MonoBehaviour
{
    public static Scheduler Instance { get; private set; }

    private readonly Dictionary<Guid, SchedulerRoutine> runningCoroutines = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    //Routine starters***********************************

    /// <summary>
    /// Delay the executon of a coroutine <paramref name="routine"/> by <paramref name="delaySeconds"/> seconds
    /// </summary>
    /// <param name="routine">Routine to start when timer is over</param>
    public Guid DelayExecution(IEnumerator routine, float delaySeconds)
    {
        return StartRoutine(new DelayerRoutine(routine, delaySeconds));
    }

    /// <summary>
    /// Delay the execution of a funtion <paramref name="function"/> by <paramref name="delaySeconds"/> seconds
    /// </summary>
    /// <param name="function">Function to call when timer is over</param>
    public Guid DelayExecution(Action function, float delaySeconds)
    {
        return StartRoutine(new DelayerRoutine(function, delaySeconds));
    }

    /// <summary>
    /// Performs a linear interpolation over a specified duration, invoking a callback upon completion.
    /// </summary>
    /// <remarks>This method starts a coroutine to perform the interpolation. The <paramref
    /// name="toLerpFunction"/> is called repeatedly  with interpolated values over the specified duration. Ensure that
    /// the method is called within a MonoBehaviour context that supports coroutines.</remarks>
    /// <param name="toLerpFunction">A function that receives the interpolated value, ranging from 0 to 1, during the interpolation process.</param>
    /// <param name="duration">The total duration of the interpolation, in seconds. Must be greater than 0.</param>
    /// <param name="callback">An action to be invoked when the interpolation completes.</param>
    public Guid Lerp(Action<float> toLerpFunction, float duration, Action callback = null)
    {
        return StartRoutine(new LerpRoutine(toLerpFunction, duration, callback));
    }    

    //Generic fucntions***********************************

    public Guid StartRoutine(SchedulerRoutine routine)
    {
        routine.handle = Guid.NewGuid();
        routine.coroutineReference = StartCoroutine(routine.Routine());
        runningCoroutines.Add(routine.handle, routine);

        Debug.Log($"Creating {routine.GetType()} with Guid {routine.handle}");

        return routine.handle;
    }
    
    public void StopTrackingRoutine(Guid hanlde) => runningCoroutines.Remove(hanlde);

    public void StopRoutine(Guid handle)
    {
        if (!runningCoroutines.ContainsKey(handle))
        {
            Debug.LogError($"Routine with Guid {handle} was not found");
            return;
        }

        Debug.Log($"Stopping routine {runningCoroutines[handle].GetType()} with Guid {runningCoroutines[handle].handle}");

        StopCoroutine(runningCoroutines[handle].coroutineReference);
        StopTrackingRoutine(handle);
    }
}

public abstract class SchedulerRoutine
{
    public Guid handle;
    public Coroutine coroutineReference;

    public abstract IEnumerator Routine();
}
