using System;
using UnityEngine;

public class BobTailState : BobState
{
    private float attackTime = 0;
    private int projectileCount = 0;
    private float attackArc;
    private GameObject needlePrefab;
    private Transform bobTransform;
    private float needleStrength = 2.5f;
    private Transform needleParentTransform;

    private float timeBetweenProjectiles = 0;
    private int firedProjectileCount = 0;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 7) throw new Exception("Provided parameters array length was not 7");

        attackTime = (float)parameters[0];
        projectileCount = (int)parameters[1];
        attackArc = (float)parameters[2];
        needlePrefab = (GameObject)parameters[3];
        bobTransform = (Transform)parameters[4];
        needleStrength = (float)parameters[5];
        needleParentTransform = (Transform)parameters[6];
    }

    public override void LoadState(params object[] parameters)
    {
        timeBetweenProjectiles = 1 / (float)projectileCount;

        isStateRunning = true;

        Scheduler.Instance.Lerp(FireProjectile, attackTime, () => isStateRunning = false);
    }

    public override void TickState()
    {
        //if (!isStateRunning) return;

        //timer += Time.deltaTime / attackTime;
        //if (timer >= (firedProjectileCount + 1) * timeBetweenProjectiles) FireProjectile();

        //if (timer >= 1) isStateRunning = false;
    }

    public override void UnloadState()
    {
        firedProjectileCount = 0;
    }

    private void FireProjectile(float t)
    {
        if (t < (firedProjectileCount + 1) * timeBetweenProjectiles) return;

        GameObject.Instantiate(needlePrefab, new Vector3(0, 1, 0), Quaternion.identity, needleParentTransform)
            .GetComponent<Rigidbody>()
            .AddForce(needleStrength * ((UnityEngine.Random.value * 0.5f) + 0.5f) * NeedleDirection(), ForceMode.Impulse);

        firedProjectileCount++;
    }

    Vector3 NeedleDirection()
    {
        var angle = attackArc / 2f;
        var dir = Quaternion.Euler(0, UnityEngine.Random.Range(-angle, angle), 0) * (-bobTransform.forward);
        return dir;
    }
}
