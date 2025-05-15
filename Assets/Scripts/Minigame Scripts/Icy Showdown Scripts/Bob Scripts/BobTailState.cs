using UnityEngine;
using System;

public class BobTailState : BobState
{
    private float attackTime = 0;
    private int projectileCount = 0;
    private Vector2 attackArc;
    private GameObject needlePrefab;
    private Transform bobTransform;

    private float timeBetweenProjectiles = 0;
    private int firedProjectileCount = 0;

    private float timer = 0;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 5) throw new Exception("Provided parameters array length was not 5");

        attackTime = (float)parameters[0];
        projectileCount = (int)parameters[1];
        attackArc = (Vector2)parameters[2];
        needlePrefab = (GameObject)parameters[3];
        bobTransform = (Transform)parameters[4];
    }

    public override void LoadState(params object[] parameters)
    {
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
        var dir = NeedleDirection();

        GameObject.Instantiate(needlePrefab).GetComponent<Rigidbody>().AddForce(dir, ForceMode.Impulse);

        firedProjectileCount++;
    }

    Vector3 NeedleDirection()
    {
        var dir = new Vector3(attackArc.x / 2f / 90f, 0, attackArc.x / 2f / 90f);

        dir.Set(
            UnityEngine.Random.Range(-dir.x, dir.x),
            UnityEngine.Random.Range(-dir.z, dir.z),
            0
        );

        dir = bobTransform.rotation * dir;

        return dir;
    }
}
