using System;
using System.Collections;
using UnityEngine;

public class LerpRoutine : SchedulerRoutine
{
    private readonly Action<float> toLerpFunction;
    private readonly float duration;
    private readonly Action callback;

    public LerpRoutine(Action<float> toLerpFunction, float duration, Action callback = null)
    {
        this.toLerpFunction = toLerpFunction;
        this.duration = duration;
        this.callback = callback;
    }

    public override IEnumerator Routine()
    {
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime / duration;
            toLerpFunction(time);

            yield return null;
        }

        try { callback?.Invoke(); }
        catch (MissingReferenceException e) { Debug.LogError(e.Message); }

        Scheduler.Instance.StopRoutine(handle);
    }
}
