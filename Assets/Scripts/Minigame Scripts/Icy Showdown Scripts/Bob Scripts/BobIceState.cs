using UnityEngine;

public class BobIceState : BobState
{
    private readonly float attackSeconds = 2;
    private readonly float attackAngle = 90;

    private Transform bobTransform;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float turnMultiplier;
    private float time;

    public override void LoadState(params object[] parameters)
    {
        bobTransform = (Transform)parameters[0];
        isStateRunning = true;
        turnMultiplier = Random.Range(0, 2) == 0 ? 1 : -1;
        initialRotation = bobTransform.rotation;
        targetRotation = initialRotation * Quaternion.Euler(0, attackAngle * turnMultiplier, 0);
    }

    public override void TickState()
    {
        if (!isStateRunning) return;
        time += Time.deltaTime / attackSeconds;
        bobTransform.rotation = Quaternion.Lerp(initialRotation, targetRotation, time);
        Fire();
        if (time >= attackSeconds) isStateRunning = false;
    }

    public override void UnloadState()
    {
        time = 0;
    }

    private void Fire()
    {
        return;
    }
}
