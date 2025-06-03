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
    private GameObject hitEffect;
    private int layerMask;
    private float stunDuration;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float turnMultiplier;
    private float time = 0;

    private IceState state = IceState.Idle;
    private GameObject instantiatedHitEffect;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 6) throw new Exception("Provided parameters array length was not 6");

        bobTransform = (Transform)parameters[0];
        chargeUpEffect = (VisualEffect)parameters[1];
        beamEffect = (VisualEffect)parameters[2];
        hitEffect = (GameObject)parameters[3];
        layerMask = (int)parameters[4];
        stunDuration = (float)parameters[5];
    }

    public override void LoadState(params object[] parameters)
    {
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
        instantiatedHitEffect.GetComponent<VisualEffect>().Stop();
        GameObject.Destroy(instantiatedHitEffect, 0.5f);
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

            Camera.main.GetComponent<PlayerCenterFollow>().ShakeCamera(attackSeconds);

            instantiatedHitEffect = GameObject.Instantiate(hitEffect);
        }
    }

    private void Fire()
    {
        time += Time.deltaTime / attackSeconds;
        RaycastAndHitParticle();
        bobTransform.rotation = Quaternion.Lerp(initialRotation, targetRotation, time);
        IcePlatformManager.Instance.ExecuteForEachPlatform(FreezePlatformInArc);
    }

    private void FreezePlatformInArc(IcePlatform platform)
    {
        var platformPosition = platform.transform.position;
        platformPosition.y = 0;
        var angleToPlatform = Vector3.Angle(bobTransform.forward, platformPosition.normalized);
        if (angleToPlatform < 1f) platform.FreezePlatform();
    }

    private void RaycastAndHitParticle()
    {
        if (Physics.Raycast(new Ray(new Vector3(0, 0.2f, 0), bobTransform.forward), out RaycastHit hit, 10f, layerMask))
        {
            instantiatedHitEffect.transform.position = hit.point;
            if (hit.collider.CompareTag("Player"))
            {
                var player = hit.collider.GetComponent<MinigamePlayer>();
                player.StunPlayer(stunDuration);
                player.DropTreasure();
            }
            else if (hit.collider.CompareTag("Icicle"))
            {
                GameObject.Destroy(hit.collider.gameObject);
            }
        }
    }
}
