using System;
using UnityEngine;
using UnityEngine.VFX;

public class BobIceState : BobState
{    private readonly float chargeupTime = 2;
    private readonly float attackSeconds = 2;
    private readonly float attackAngle = 90;

    private Transform bobTransform;
    private VisualEffect chargeUpEffect;
    private VisualEffect beamEffect;
    private GameObject hitEffect;
    private int layerMask;
    private float pushForce = 20;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float turnMultiplier;

    private Animator animator;
    private GameObject instantiatedHitEffect;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 5) throw new Exception("Provided parameters array length was not 6");

        bobTransform = (Transform)parameters[0];
        chargeUpEffect = (VisualEffect)parameters[1];
        beamEffect = (VisualEffect)parameters[2];
        hitEffect = (GameObject)parameters[3];
        layerMask = (int)parameters[4];

        animator = Bob.Instance.Animator;
    }

    public override void LoadState(params object[] parameters)
    {
        ChoseTargetRotation();

        isStateRunning = true;
        chargeUpEffect.Play();
        AudioManager.PlaySound(ESoundType.Bob, "Laser", false);

        animator.SetFloat("BigBeam", 0f);
        animator.SetFloat("SpeedMult", 1 / chargeupTime);
        animator.SetTrigger("BeamAttack");

        Scheduler.Instance.DelayExecution(ChargeUp, chargeupTime);
    }

    public override void TickState()
    {

    }

    public override void UnloadState()
    {
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
        chargeUpEffect.Stop();
        beamEffect.Play();

        Camera.main.GetComponent<PlayerCenterFollow>().ShakeCamera(attackSeconds);

        instantiatedHitEffect = GameObject.Instantiate(hitEffect);
        animator.SetFloat("SpeedMult", 1 / attackSeconds);
        animator.SetTrigger("Advance");
        Scheduler.Instance.Lerp(Fire, attackSeconds, () =>
        {
            isStateRunning = false;
            beamEffect.Stop();
        });
    }

    private void Fire(float t)
    {
        RaycastAndHitParticle();
        bobTransform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
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
                player.GetPlayerAnimator.SetTrigger("Hit");
                player.PushPlayer(DifficultyManager.IsEasyMode() ? pushForce * 0.75f : pushForce);
            }
            else if (hit.collider.CompareTag("Icicle"))
            {
                GameObject.Destroy(hit.collider.gameObject);
            }
        }
    }
}
