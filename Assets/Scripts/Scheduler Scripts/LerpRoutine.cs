using System;
using System.Collections;
using UnityEngine;

public class LerpRoutine : SchedulerRoutine
{
    public override IEnumerator Routine(params object[] args)
    {
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime / (float)args[1];
            ((Action<float>)args[0])(time);

            yield return null;
        }

        try { ((Action)args[2])?.Invoke(); }
        catch (MissingReferenceException e) { Debug.LogError(e.Message); }

        Scheduler.Instance.StopRoutine(handle);
    }
}
