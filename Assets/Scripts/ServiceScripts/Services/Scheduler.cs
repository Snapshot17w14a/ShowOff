using System;
using System.Collections;
using UnityEngine;

public class Scheduler : MonoBehaviour
{
    public static Scheduler Instance { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    /// <summary>
    /// Delay the executon of a coroutine <paramref name="routine"/> by <paramref name="delaySeconds"/> seconds
    /// </summary>
    /// <param name="routine">Routine to start when timer is over</param>
    public void DelayExecution(IEnumerator routine, float delaySeconds)
    {
        StartCoroutine(DelayedRoutine(routine, delaySeconds));
    }

    /// <summary>
    /// Delay the execution of a funtion <paramref name="function"/> by <paramref name="delaySeconds"/> seconds
    /// </summary>
    /// <param name="function">Function to call when timer is over</param>
    public void DelayExecution(Action function, float delaySeconds)
    {
        StartCoroutine(DelayedRoutine(function, delaySeconds));
    }

    /// <summary>
    /// Performs a linear interpolation over a specified duration, invoking a callback upon completion.
    /// </summary>
    /// <remarks>This method starts a coroutine to perform the interpolation. The <paramref
    /// name="toLerpFunction"/> is called repeatedly  with interpolated values over the specified duration. Ensure that
    /// the method is called within a MonoBehaviour context  that supports coroutines.</remarks>
    /// <param name="toLerpFunction">A function that receives the interpolated value, ranging from 0 to 1, during the interpolation process.</param>
    /// <param name="duration">The total duration of the interpolation, in seconds. Must be greater than 0.</param>
    /// <param name="callback">An action to be invoked when the interpolation completes.</param>
    public void Lerp(Action<float> toLerpFunction, float duration, Action callback = null)
    {
        StartCoroutine(LerpRoutine(toLerpFunction, duration, callback));
    }    

    private IEnumerator DelayedRoutine(IEnumerator routine, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        StartCoroutine(routine);
    }

    private IEnumerator DelayedRoutine(Action routine, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        routine();
    }

    private IEnumerator LerpRoutine(Action<float> toLerpFunction, float duration, Action callback)
    {
        float time = 0;

        while(time < 1)
        {
            time += Time.deltaTime / duration;
            toLerpFunction(time);

            yield return null;
        }

        callback?.Invoke();
    }
}
