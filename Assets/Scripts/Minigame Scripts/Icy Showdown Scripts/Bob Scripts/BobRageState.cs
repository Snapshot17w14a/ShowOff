using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using Random = UnityEngine.Random;
public class BobRageState : BobState
{
    private float duration = 0;
    private float chargeUpTime = 0;
    private float stunDuration = 0;
    private Transform bobTransform;
    private VisualEffect beamsEffect;
    private int layerMask;
    private Volume globalVolume;
    private ChromaticAberration chromatic;
    private Camera mainCamera;
    private int pushForce = 20;

    private float initialAngle = 0f;
    private float targetAngle = 0f;

    private Animator animator;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 7) throw new Exception("Provided parameters array length was not 7");

        duration = (float)parameters[0];
        bobTransform = (Transform)parameters[1];
        beamsEffect = (VisualEffect)parameters[2];
        chargeUpTime = (float)parameters[3];
        stunDuration = (float)parameters[4];
        layerMask = (int)parameters[5];
        globalVolume = (Volume)parameters[6];

        animator = Bob.Instance.Animator;
        mainCamera = Camera.main;
    }

    public override void LoadState(params object[] parameters)
    {
        initialAngle = bobTransform.eulerAngles.y;
        targetAngle = initialAngle + (Random.Range(0, 2) == 0 ? -360f : 360f);

        beamsEffect.Reinit();
        beamsEffect.Play();
        AudioManager.PlaySound(ESoundType.Bob, "Quad_Lasers", false);

        globalVolume.sharedProfile.TryGet(out chromatic);

        isStateRunning = true;

        animator.SetFloat("BigBeam", 1f);
        animator.SetFloat("SpeedMult", 1 / chargeUpTime);
        animator.SetTrigger("BeamAttack");

        Scheduler.Instance.Lerp(t => chromatic.intensity.value = Mathf.Lerp(0, 0.5f, t), chargeUpTime, StartRagePhase);
    }

    public override void TickState()
    {
        
    }

    public override void UnloadState()
    {
        beamsEffect.Stop();
        mainCamera.fieldOfView = 26.9f;
    }

    private void StartRagePhase()
    {
        animator.SetFloat("SpeedMult", 1 / duration * 4);
        animator.SetTrigger("Advance");
        Scheduler.Instance.Lerp(RagePhase, duration, EndAttack);
    }

    private void RagePhase(float t)
    {
        Rotate(t);
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 30f, Time.deltaTime);
        foreach (var direction in new Vector3[] { bobTransform.forward, bobTransform.right, -bobTransform.forward, -bobTransform.right })
        {
            Raycast(direction);
            IcePlatformManager.Instance.ExecuteForEachPlatform(platform => FreezePlatformInArc(platform, direction));
        }
    }

    private void EndAttack()
    {
        animator.SetTrigger("EndBeam");
        Scheduler.Instance.Lerp(
            t => {
                mainCamera.fieldOfView = Mathf.Lerp(30f, 26.9f, t);
                chromatic.intensity.value = Mathf.Lerp(0.5f, 0, t);
            }, 
            1, 
            () => isStateRunning = false
        );
    }

    private void Rotate(float t)
    {
        bobTransform.eulerAngles = new Vector3(0, Mathf.Lerp(initialAngle, targetAngle, t), 0);
    }

    private void Raycast(Vector3 dir)
    {
        if (Physics.Raycast(new Vector3(0, 0.2f, 0), dir, out RaycastHit hit, 10f, layerMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                var player = hit.collider.GetComponent<MinigamePlayer>();
                player.PushPlayer(pushForce);
                player.GetPlayerAnimator.SetTrigger("Hit");
                /*player.StunPlayer(stunDuration);
                player.DropTreasure();*/
            }
            else if (hit.collider.CompareTag("Icicle"))
            {
                GameObject.Destroy(hit.collider.gameObject);
            }
        }
    }

    private void FreezePlatformInArc(IcePlatform platform, Vector3 dir)
    {
        var platformPosition = platform.transform.position;
        platformPosition.y = 0;
        var angleToPlatform = Vector3.Angle(dir, platformPosition.normalized);
        if (angleToPlatform < 1f) platform.FreezePlatform();
    }
}
