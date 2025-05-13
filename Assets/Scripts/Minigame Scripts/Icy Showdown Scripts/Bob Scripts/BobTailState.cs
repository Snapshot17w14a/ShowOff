using UnityEngine;
using System;

public class BobTailState : BobState
{
    private float attackTime = 0;
    private int projectileCount = 0;

    private float timeBetweenProjectiles = 0;
    private int firedProjectileCount = 0;

    private float timer = 0;

    public override void LoadState(params object[] parameters)
    {
        if (parameters.Length != 2) throw new Exception("Provided parameters array length was not 2");

        attackTime = (float)parameters[0];
        projectileCount = (int)parameters[1];

        timeBetweenProjectiles = 1 / (float)projectileCount;

        isStateRunning = true;
    }

    public override void TickState()
    {
        if (!isStateRunning) return;

        timer += Time.deltaTime / attackTime;
        if (timer >= (firedProjectileCount + 1) * timeBetweenProjectiles) FireProjectile();

        if (timer >= 1) isStateRunning = false;
    }

    public override void UnloadState()
    {
        timer = 0;
        firedProjectileCount = 0;
    }

    private void FireProjectile()
    {
        //Debug.Log("Fired spruce needle");
        firedProjectileCount++;
    }
}
