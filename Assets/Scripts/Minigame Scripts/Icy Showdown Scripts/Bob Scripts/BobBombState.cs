using System;
using UnityEngine;

public class BobBombState : BobState
{
    private GameObject bombPrefab;
    private Transform bombParentTransform;

    public override void LoadState(params object[] parameters)
    {
        if (parameters.Length != 2) throw new Exception("Provided parameters array length was not 2");

        bombPrefab = (GameObject)parameters[0];
        bombParentTransform = (Transform)parameters[1];

        isStateRunning = true;

        var targetPlatform = IcePlatformManager.Instance.GetRandomPlatform;
        LaunchBomb(Vector3.zero, targetPlatform);
    }

    public override void TickState()
    {
        if (!isStateRunning) return;
    }

    public override void UnloadState()
    {
        
    }

    public void LaunchBomb(Vector3 start, IcePlatform target, float timeToTarget = 2f)
    {
        var bomb = GameObject.Instantiate(bombPrefab, bombParentTransform);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        var bombScript = bomb.GetComponent<BobBomb>();
        bombScript.onBombExplode = BombCallback;
        bombScript.targetPlatform = target;

        Vector3 displacement = target.transform.position - start;
        Vector3 horizontalDisplacement = new(displacement.x, 0, displacement.z);

        float horizontalDistance = horizontalDisplacement.magnitude;
        float verticalDistance = displacement.y;

        // Horizontal speed needed to reach the target in time
        float horizontalSpeed = horizontalDistance / timeToTarget;

        // Vertical speed needed to reach the height in time (accounting for gravity)
        float verticalSpeed = (verticalDistance - 0.5f * Physics.gravity.y * timeToTarget * timeToTarget) / timeToTarget;

        // Final velocity vector
        Vector3 direction = horizontalDisplacement.normalized;
        Vector3 velocity = direction * horizontalSpeed;
        velocity.y = verticalSpeed;

        // Set the new Unity 6 style linear velocity
        rb.linearVelocity = velocity;
    }

    private void BombCallback()
    {
        isStateRunning = false;
    }
}
