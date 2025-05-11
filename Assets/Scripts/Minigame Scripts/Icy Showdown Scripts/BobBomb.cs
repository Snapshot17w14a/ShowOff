using System;
using UnityEngine;

public class BobBomb : MonoBehaviour
{
    enum BombState { Flying, WaitingToExplode }

    public Action onBombExplode;
    public IcePlatform targetPlatform;
    /*[HideInInspector]*/ private float flightTime = 2f;
    /*[HideInInspector]*/ private float explosionCooldown = 3f;

    private BombState state = BombState.Flying;
    private float timer = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case BombState.Flying:
                if (timer >= flightTime)
                {
                    timer -= flightTime;
                    state = BombState.WaitingToExplode;
                    GetComponent<Rigidbody>().isKinematic = true;
                }
            break;
            case BombState.WaitingToExplode:
                if (timer >= explosionCooldown)
                {
                    Explode();
                }
            break;
        }
    }

    private void Explode()
    {
        onBombExplode();
        targetPlatform.BreakPlatform();
        Destroy(gameObject);
    }
}
