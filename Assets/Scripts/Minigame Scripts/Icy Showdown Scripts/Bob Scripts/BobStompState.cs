using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BobStompState : BobState
{
    private const float Tau = 2 * Mathf.PI;

    private GameObject iciclePrefab;
    private float radius;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 2) throw new Exception("Provided parameters array length was not 2");

        iciclePrefab = (GameObject)parameters[0];
        radius = (float)parameters[1];
    }

    public override void LoadState(params object[] parameters)
    {
        isStateRunning = true;
        SpawnIcicles(UnityEngine.Random.Range(2, 5));
        IcePlatformManager.Instance.BreakBrittlePlatforms();
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
            var dir = new Vector3(Mathf.Cos(Random.value * Tau), 0, Mathf.Sin(Random.value * Tau));
            var pos = dir * radius;
            pos.y = 10f;

            GameObject.Instantiate(iciclePrefab, pos, Quaternion.identity);
        }

        isStateRunning = false;
    }
}
