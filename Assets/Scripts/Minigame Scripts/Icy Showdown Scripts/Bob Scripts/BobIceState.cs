using UnityEngine;
using System;

public class BobIceState : BobState
{
    private readonly float attackSeconds = 2;
    private readonly float attackAngle = 90;

    private Transform bobTransform;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float turnMultiplier;
    private float time = 0;

    public override void LoadState(params object[] parameters)
    {
        if (parameters.Length != 1) throw new Exception("Provided parameters array length was not 1");

        bobTransform = (Transform)parameters[0];

        ChoseTargetRotation();

        isStateRunning = true;
    }

    public override void TickState()
    {
        if (!isStateRunning) return;
        time += Time.deltaTime / attackSeconds;
        bobTransform.rotation = Quaternion.Lerp(initialRotation, targetRotation, time);
        Fire();
        if (time >= 1) isStateRunning = false;
    }

    public override void UnloadState()
    {
        time = 0;
    }

    private void ChoseTargetRotation()
    {
        turnMultiplier = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
        initialRotation = bobTransform.rotation;
        targetRotation = initialRotation * Quaternion.Euler(0, attackAngle * turnMultiplier, 0);
    }

    private void Fire()
    {
        IcePlatformManager.Instance.ExecuteForEachPlatform(FreezePlatformInArc);
    }

    private void FreezePlatformInArc(IcePlatform platform)
    {
        var angleToPlatform = Vector3.Angle(bobTransform.forward, (platform.transform.position - bobTransform.position).normalized);
        if (angleToPlatform < 1f) platform.FreezePlatform();
    }
}
