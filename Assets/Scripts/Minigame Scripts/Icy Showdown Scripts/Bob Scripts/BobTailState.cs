using UnityEngine;
using System;
using UnityEngine.VFX;

public class BobTailState : BobState
{
    private float attackTime = 0;
    private int projectileCount = 0;
    private float attackArc;
    private GameObject needlePrefab;
    private Transform bobTransform;
    private float needleStrength = 2.5f;
    private Transform needleParentTransform;
    private VisualEffect stompEffect;

    private float timeBetweenProjectiles = 0;
    private int firedProjectileCount = 0;

    private float timer = 0;

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
        var dir = NeedleDirection();

        GameObject.Instantiate(needlePrefab, new Vector3(0, 1, 0), Quaternion.identity, needleParentTransform).GetComponent<Rigidbody>().AddForce(needleStrength * ((UnityEngine.Random.value * 0.5f) + 0.5f) * dir, ForceMode.Impulse);

        firedProjectileCount++;
    }

    Vector3 NeedleDirection()
    {
        //var dir = new Vector3(attackArc.x / 2f / 90f, 0, attackArc.y / 2f / 90f);

        //dir.Set(
        //    UnityEngine.Random.Range(-dir.z, dir.z),
        //    UnityEngine.Random.Range(-dir.x, dir.x),
        //    1  
        //);

        //dir = bobTransform.rotation * dir;

        //return -dir;

        var angle = attackArc / 2f;
        var dir = Quaternion.Euler(0, UnityEngine.Random.Range(-angle, angle), 0) * (-bobTransform.forward);
        return dir;
    }
}
