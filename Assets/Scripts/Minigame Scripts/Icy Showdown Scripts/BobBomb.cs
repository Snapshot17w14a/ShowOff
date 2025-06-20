using System;
using UnityEngine;
using UnityEngine.VFX;

public class BobBomb : MonoBehaviour
{
    enum BombState { Flying, WaitingToExplode }

    public Action onBombExplode;

    public IcePlatform targetPlatform;

    [SerializeField] private VisualEffect bombExplosion;
    [SerializeField] private VisualEffect trailEffect;

    private float flightTime = 2f;
    private float explosionCooldown = 3f;

    private BombState state = BombState.Flying;
    private float timer = 0;

    private Rigidbody rb;

    private void Start()
    {
        bombExplosion.SetFloat("Lifetime", explosionCooldown + flightTime);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case BombState.Flying:
                transform.up = rb.linearVelocity.normalized;

                if (timer >= flightTime)
                {
                    timer -= flightTime;
                    state = BombState.WaitingToExplode;
                    trailEffect.Stop();
                    rb.isKinematic = true;
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
        transform.GetChild(0).transform.SetParent(transform.parent);
        Destroy(gameObject);
    }
}
