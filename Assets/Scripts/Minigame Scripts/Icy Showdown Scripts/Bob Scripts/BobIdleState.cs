using UnityEngine;
using System;

public class BobIdleState : BobState
{
    private float idleSeconds;
    private float time = 0;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private Transform bobTransform;

    public override void LoadState(params object[] parameters)
    {
        if (parameters.Length != 2) throw new Exception("Provided parameters array length was not 2");

        bobTransform = (Transform)parameters[0];
        idleSeconds = (float)parameters[1];

        ChooseRandomAngle();

        isStateRunning = true;
    }

    public override void TickState()
    {
        if (!isStateRunning) return;

        time += Time.deltaTime / idleSeconds;
        bobTransform.rotation = Quaternion.Lerp(initialRotation, targetRotation, time);

        if (time >= 1) isStateRunning = false;
    }

    public override void UnloadState()
    {
        time = 0;
    }

    private void ChooseRandomAngle()
    {
        float angle = UnityEngine.Random.Range(0.0f, 1.0f);
        targetRotation = Quaternion.Euler(0, angle * 360f, 0);

        initialRotation = bobTransform.rotation;
    }
}
