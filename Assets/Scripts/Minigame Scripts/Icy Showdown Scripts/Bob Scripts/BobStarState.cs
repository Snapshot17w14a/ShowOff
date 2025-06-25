using System;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class BobStarState : BobState
{
    private float timeTillSpawn;
    private GameObject iciclePrefab;
    private int minCount;
    private int maxCount;
    private VisualEffect starEffect;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 5) throw new Exception("Provided parameters array length was not 4");

        timeTillSpawn = (float)parameters[0];
        iciclePrefab = (GameObject)parameters[1];
        minCount = (int)parameters[2];
        maxCount = (int)parameters[3];
        starEffect = (VisualEffect)parameters[4];
    }

    public override void LoadState(params object[] parameters)
    {
        isStateRunning = true;
        starEffect.Reinit();
        starEffect.Play();

        Scheduler.Instance.DelayExecution(() => SpawnIcicles(Random.Range(minCount, maxCount + 1)), timeTillSpawn);
    }

    public override void TickState()
    {
    }

    public override void UnloadState()
    {
    }

    private void SpawnIcicles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Scheduler.Instance.DelayExecution(() =>
            {
                var randDir = Random.insideUnitCircle;
                var dir = new Vector3(randDir.x, 0, randDir.y);
                var pos = dir * 2;
                pos.y = 10f;

                GameObject.Instantiate(iciclePrefab, pos, Quaternion.identity);

                if (i == count) isStateRunning = false;
            }, i * 0.4f);
        }
        AudioManager.PlaySound(ESoundType.Bob, "Tail_Star_Shoot", false);
    }
}
