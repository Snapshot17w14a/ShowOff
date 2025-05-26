using System;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class BobStompState : BobState
{
    private const float Tau = 2 * Mathf.PI;

    private GameObject iciclePrefab;
    private float radius;
    private float knockbackRange;
    private float knockbackForce;
    private VisualEffect stompEffect;

    public override void Initialize(params object[] parameters)
    {
        if (parameters.Length != 5) throw new Exception("Provided parameters array length was not 5");

        iciclePrefab = (GameObject)parameters[0];
        radius = (float)parameters[1];
        knockbackRange = (float)parameters[2];
        knockbackForce = (float)parameters[3];
        stompEffect = (VisualEffect)parameters[4];
    }

    public override void LoadState(params object[] parameters)
    {
        isStateRunning = true;
        SpawnIcicles(Random.Range(2, 5));
        KnockBackPlayers();
        stompEffect.Play();
        IcePlatformManager.Instance.BreakBrittlePlatforms();
        Camera.main.GetComponent<PlayerCenterFollow>().ShakeCamera(1f);
    }

    public override void TickState()
    {
        
    }

    public override void UnloadState()
    {
        stompEffect.Stop();
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

    private void KnockBackPlayers()
    {
        var players = GameObject.FindObjectsByType<MinigamePlayer>(FindObjectsSortMode.None);

        foreach(var p in players)
        {
            var pos = p.transform.position;
            var dist = pos.magnitude;

            if (dist < knockbackRange)
            {
                p.GetComponent<Rigidbody>().AddForce(pos.normalized * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}
