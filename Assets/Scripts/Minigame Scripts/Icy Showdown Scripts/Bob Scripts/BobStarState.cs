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

    private float time = 0;

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
    }

    public override void TickState()
    {
        time += Time.deltaTime;

        if (time >= timeTillSpawn) SpawnIcicles(Random.Range(minCount, maxCount + 1));
    }

    public override void UnloadState()
    {
        time = 0;
    }

    private void SpawnIcicles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var randDir = Random.insideUnitCircle;
            var dir = new Vector3(randDir.x, 0, randDir.y);
            var pos = dir * 2;
            pos.y = 10f;

            GameObject.Instantiate(iciclePrefab, pos, Quaternion.identity);
        }

        isStateRunning = false;
    }
}
