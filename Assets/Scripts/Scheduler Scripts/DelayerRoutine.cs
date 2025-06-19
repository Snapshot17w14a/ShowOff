using System;
using System.Collections;
using UnityEngine;

public class DelayerRoutine : SchedulerRoutine
{
    private readonly object callback;
    private readonly float delaySeconds;

    public DelayerRoutine(SchedulerRoutine scheduledRoutine, float delaySeconds)
    {
        callback = scheduledRoutine;
        this.delaySeconds = delaySeconds;
    }

    public DelayerRoutine(Action scheduledMethod, float delaySeconds)
    {
        callback = scheduledMethod;
        this.delaySeconds = delaySeconds;
    }

    public DelayerRoutine(IEnumerator scheduledCoroutine, float delaySeconds)
    {
        callback = scheduledCoroutine;
        this.delaySeconds = delaySeconds;
    }

    public override IEnumerator Routine()
    {
        yield return new WaitForSeconds(delaySeconds);

        if (callback is Action func) func();
        else if (callback is SchedulerRoutine routine) Scheduler.Instance.StartCoroutine(routine.Routine());
        else if (callback is IEnumerator coroutine) Scheduler.Instance.StartCoroutine(coroutine);

        Scheduler.Instance.StopRoutine(handle);
    }
}