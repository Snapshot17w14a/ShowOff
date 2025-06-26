using System;
using UnityEngine;

public class BobIdleState : BobState
{
    private float idleSeconds;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private Transform bobTransform;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 1) throw new Exception("Provided parameters array length was not 1");

        bobTransform = (Transform)parameters[0];
    }

    public override void LoadState(params object[] parameters)
    {
        idleSeconds = (float)parameters[0];

        ChooseRandomAngle();

        isStateRunning = true;

        float angleBetween = Quaternion.Angle(bobTransform.rotation, targetRotation);
        var bobAnimator = Bob.Instance.Animator;
        bobAnimator.SetFloat("Move", Mathf.Sign(angleBetween));
        bobAnimator.SetTrigger("MoveTrigger");

        Scheduler.Instance.Lerp(t => bobTransform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t), 
            idleSeconds, 
            () =>
            {
                isStateRunning = false;
                bobAnimator.SetFloat("Move", 0);
            });
    }

    public override void TickState()
    {
    }

    public override void UnloadState()
    {
    }


    private void ChooseRandomAngle()
    {
        float angle = UnityEngine.Random.Range(0.0f, 1.0f);
        targetRotation = Quaternion.Euler(0, angle * 360f, 0);

        initialRotation = bobTransform.rotation;
    }
}
