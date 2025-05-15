using UnityEngine.VFX;
using UnityEngine;
using System;

public class BobIceState : BobState
{
    private enum IceState { ChargeUp, Firing, Idle };

    private readonly float chargeupTime = 2;
    private readonly float attackSeconds = 2;
    private readonly float attackAngle = 90;

    private Transform bobTransform;
    private VisualEffect chargeUpEffect;
    private VisualEffect beamEffect;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float turnMultiplier;
    private float time = 0;

    private IceState state = IceState.Idle;

    public override void LoadState(params object[] parameters)
    {
        if (parameters.Length != 3) throw new Exception("Provided parameters array length was not 3");

        bobTransform = (Transform)parameters[0];
        chargeUpEffect = (VisualEffect)parameters[1];
        beamEffect = (VisualEffect)parameters[2];

        ChoseTargetRotation();

        isStateRunning = true;

        //Set up ChargeUp state
        state = IceState.ChargeUp;
        chargeUpEffect.Play();
    }

    public override void TickState()
    {
        if (!isStateRunning) return;
        
        switch (state)
        {
            case IceState.Idle:
                return;
            case IceState.ChargeUp:
                ChargeUp();
                break;
            case IceState.Firing:
                Fire();
                break;
        }

        if (state == IceState.Firing && time >= 1)
        {
            isStateRunning = false;
            beamEffect.Stop();
        }
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

    private void ChargeUp()
    {
        time += Time.deltaTime / chargeupTime;
        if (time >= 1)
        {
            time = 0;
            state = IceState.Firing;
            chargeUpEffect.Stop();
            beamEffect.Play();
        }
    }

    private void Fire()
    {
        time += Time.deltaTime / attackSeconds;
        bobTransform.rotation = Quaternion.Lerp(initialRotation, targetRotation, time);
        IcePlatformManager.Instance.ExecuteForEachPlatform(FreezePlatformInArc);
    }

    private void FreezePlatformInArc(IcePlatform platform)
    {
        var platformPosition = platform.transform.position;
        platformPosition.y = 0;
        var angleToPlatform = Vector3.Angle(bobTransform.forward, (platformPosition - Vector3.zero).normalized);
        if (angleToPlatform < 1f) platform.FreezePlatform();
    }
}
