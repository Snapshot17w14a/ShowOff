using System;
using UnityEngine;

public enum PickupType
{
    Small,
    Large
}

public class Pickupable : MonoBehaviour
{
    public event Action<Pickupable> OnPickupableDespawnedEvent;
    public event Action<Pickupable, int> OnPickupableEnteredMinecartEvent;

    public PickupType PickupType => pickupType;
    public int Worth => worth;
    public bool IsPickedUp {  get; private set; } = false;

    [SerializeField] private PickupType pickupType;
    [SerializeField] private int gemPickUpSize = 12;
    [SerializeField] private int worth = 1;
   
    private float despawnTime;
    private bool isDespawning;

    private Rigidbody rb;
    private Collider collider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (isDespawning && Time.time >= despawnTime)
        {
            Despawn();
        }
    }

    public void DespawnAfter(float timeUntilDespawn)
    {
        despawnTime = Time.time + timeUntilDespawn;
        isDespawning = true;
    }

    public void Collect(Transform parent)
    {
        IsPickedUp = true;
        rb.isKinematic = true;
        gameObject.transform.SetParent(parent, false);
        //This is fucking ugly, but I was over it making it work with math
        transform.localScale = new Vector3(gemPickUpSize, gemPickUpSize, gemPickUpSize);
        gameObject.transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        IsPickedUp = false;
        transform.SetParent(null);
    }



    private void Despawn()
    {
        isDespawning = false;
        Destroy(gameObject);
        OnPickupableDespawnedEvent?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<IcePlatform>() != null)
        {
            rb.isKinematic = true;
        }

        if(other.GetComponent<Minecart>() != null)
        {
            Minecart minecart = other.GetComponent<Minecart>();
            minecart.AddGem();
            OnPickupableEnteredMinecartEvent?.Invoke(this, worth);
            Despawn();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        collider.isTrigger = true;
    }

    public void CalculateVelocity(Vector3 spawnPoint, Vector3 position, float speed)
    {
        rb.linearVelocity = PathCalculator.CalculateRequiredVelocity(spawnPoint, position, speed);
    }

    public void SetKinematic(bool state)
    {
        rb.isKinematic = state;
    }

    public void SetTrigger(bool state)
    {
        collider.isTrigger = state;
    }
}
