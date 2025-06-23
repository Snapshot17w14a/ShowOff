using System;
using System.Collections;
using UnityEngine;

public enum PickupType
{
    Small,
    Large
}

public enum ColliderState
{
    Flying,
    Grounded
}

public class Pickupable : MonoBehaviour
{
    public event Action<Pickupable> OnPickupableDespawnedEvent;

    public PickupType PickupType => pickupType;
    public int Worth => worth;
    public bool IsPickedUp { get; private set; } = false;

    [SerializeField] private Material[] gemMaterials;
    [SerializeField] private PickupType pickupType;
    [SerializeField] private int gemPickUpSize = 12;
    [SerializeField] private int worth = 1;

    [SerializeField] private Collider groundedCollider;
    [SerializeField] private Collider flyingCollider;

    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private Coroutine despawnCoroutine;

    public Minecart targetMinecart;
    public TreasureInteraction parentInteration;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void DespawnAfter(float time)
    {
        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
        }

        despawnCoroutine = StartCoroutine(DespawnCoroutine(time));
    }

    private IEnumerator DespawnCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        OnPickupableDespawnedEvent?.Invoke(this);
        Destroy(gameObject);
    }

    public void CancelDespawn()
    {
        if (despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
            despawnCoroutine = null;
        }
    }

    public void Collect(Transform parent)
    {
        CancelDespawn();
        IsPickedUp = true;
        SetKinematic(true);
        gameObject.transform.SetParent(parent, false);
        //This is fucking ugly, but I was over it making it work with math
        transform.localScale = new Vector3(gemPickUpSize, gemPickUpSize, gemPickUpSize);
        gameObject.transform.localPosition = Vector3.zero;
        groundedCollider.enabled = false;
        EventBus<PickupCollected>.RaiseEvent(new PickupCollected(this));
    }

    private void Despawn()
    {

        Destroy(gameObject);
        OnPickupableDespawnedEvent?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IcePlatform>() != null || other.CompareTag("IcePlatform"))
        {
            SetKinematic(true);
            flyingCollider.enabled = false;
            groundedCollider.enabled = true;
        }

        else if (other.TryGetComponent<GemCollector>(out var gemCollector))
        {
            if (gemCollector.parentMinecart.IsFull)
            {
                Despawn();
                return;
            }
            gemCollector.parentMinecart.AddGem();
            parentInteration.HandleTreasureEnteredMinecart(this, worth);
            Despawn();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        SetKinematic(true);
        flyingCollider.enabled = false;
        groundedCollider.enabled = true;
    }

    public void CalculateVelocity(Vector3 spawnPoint, Vector3 position, float speed)
    {
        rb.linearVelocity = PathCalculator.CalculateRequiredVelocity(spawnPoint, position, speed);
    }

    public void SetKinematic(bool state)
    {
        rb.isKinematic = state;
    }

    public void SetTrigger(ColliderState collider, bool state)
    {
        switch (collider)
        {
            case ColliderState.Flying:
                flyingCollider.isTrigger = state;
                break;
            case ColliderState.Grounded:
                groundedCollider.isTrigger = state;
                break;
        }
    }

    public void SetCollider(ColliderState collider, bool state)
    {
        switch (collider)
        {
            case ColliderState.Flying:
                flyingCollider.enabled = state;
                break;
            case ColliderState.Grounded:
                groundedCollider.enabled = state;
                break;
        }
    }

    public void SetRandomMaterial()
    {
        if (gemMaterials != null && gemMaterials.Length < 1)
        {
            return;
        }

        Material randomMaterial = gemMaterials[UnityEngine.Random.Range(0, gemMaterials.Length)];
        meshRenderer.material = randomMaterial;
    }
}
