using System;
using UnityEngine;
using UnityEngine.VFX;

public class BobRageState : BobState
{
    private float duration = 0;
    private float chargeUpTime = 0;
    private float fullDuration = 0;
    private float stunDuration = 0;
    private Transform bobTransform;
    private VisualEffect beamsEffect;
    private int layerMask;

    private float time = 0f;
    private float initialAngle = 0f;
    private float targetAngle = 0f;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 6) throw new Exception("Provided parameters array length was not 6");

        duration = (float)parameters[0];
        bobTransform = (Transform)parameters[1];
        beamsEffect = (VisualEffect)parameters[2];
        chargeUpTime = (float)parameters[3];
        stunDuration = (float)parameters[4];
        layerMask = (int)parameters[5];

        fullDuration = duration + chargeUpTime;
    }

    public override void LoadState(params object[] parameters)
    {

        initialAngle = bobTransform.eulerAngles.y;
        targetAngle = initialAngle + 360f;

        beamsEffect.Reinit();
        beamsEffect.Play();

        isStateRunning = true;
    }

    public override void TickState()
    {
        if (!isStateRunning) return;

        time += Time.deltaTime;

        if (time > chargeUpTime)
        {
            Rotate();
            foreach (var direction in new Vector3[] { bobTransform.forward, bobTransform.right, -bobTransform.forward, -bobTransform.right })
            {
                Raycast(direction);
                IcePlatformManager.Instance.ExecuteForEachPlatform(platform => FreezePlatformInArc(platform, direction));
            }
        }

        if (time >= fullDuration) isStateRunning = false;
    }

    public override void UnloadState()
    {
        time = 0;
        beamsEffect.Stop();
    }

    private void Rotate()
    {
        bobTransform.eulerAngles = new Vector3(0, Mathf.Lerp(initialAngle, targetAngle, (time - chargeUpTime) / duration), 0);
    }

    private void Raycast(Vector3 dir)
    {
        if (Physics.Raycast(new Vector3(0, 0.2f, 0), dir, out RaycastHit hit, 10f, layerMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                var player = hit.collider.GetComponent<MinigamePlayer>();
                player.StunPlayer(stunDuration);
                player.DropTreasure();
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
