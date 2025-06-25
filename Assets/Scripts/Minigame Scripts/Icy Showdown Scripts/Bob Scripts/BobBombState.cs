using System;
using UnityEngine;

public class BobBombState : BobState
{
    private GameObject bombPrefab;
    private Transform bombParentTransform;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 2) throw new Exception("Provided parameters array length was not 2");

        bombPrefab = (GameObject)parameters[0];
        bombParentTransform = (Transform)parameters[1];
    }

    public override void LoadState(params object[] parameters)
    {
        isStateRunning = true;

        var bombCount = DifficultyManager.IsEasyMode() ? 1 : UnityEngine.Random.Range(1, 3);
        var platforms = IcePlatformManager.Instance.SelectUniquePlatforms(bombCount);
        for (int i = 0; i < bombCount; i++) if (platforms[i] != null) LaunchBomb(Vector3.up, platforms[i]);
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
        //Instantiate the bomb prefab with the parent transform
        var bomb = GameObject.Instantiate(bombPrefab, start, Quaternion.identity, bombParentTransform);

        //Get a reference to its rigidbody
        Rigidbody rb = bomb.GetComponent<Rigidbody>();

        //Get a reference to its BobBomb script, set the callback and the target platform
        var bombScript = bomb.GetComponent<BobBomb>();
        bombScript.onBombExplode = BombCallback;
        bombScript.targetPlatform = target;

        //Calculate the reguired velocity reach the target in timeToTarget time
        rb.linearVelocity = PathCalculator.CalculateRequiredVelocity(start, target.transform.position, timeToTarget);
        AudioManager.PlaySound(ESoundType.Bob, "Bomb_Throw", false);
        AudioManager.PlaySound(ESoundType.Bob, "Bomb", false);
    }

    private void BombCallback()
    {
        Camera.main.GetComponent<PlayerCenterFollow>().ShakeCamera(1f);
        isStateRunning = false;
    }
}
