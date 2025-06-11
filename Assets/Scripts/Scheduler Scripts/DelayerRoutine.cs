using System;
using System.Collections;
using UnityEngine;

public class DelayerRoutine : SchedulerRoutine
{
    public override IEnumerator Routine(params object[] args)
    {
        yield return new WaitForSeconds((float)args[1]);

        if (args[0] is Action func) func();
        if (args[0] is SchedulerRoutine routine) Scheduler.Instance.StartCoroutine(routine.Routine());

        Scheduler.Instance.StopRoutine(handle);
    }
}