using System;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public event Action<Pickupable> OnPickupableDespawnedEvent;
    public event Action<Pickupable> OnPickupableEnteredMinecartEvent;

    public bool IsPickedUp {  get; private set; } = false;

    [SerializeField] private int gemPickUpSize = 12;
   
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
            OnPickupableEnteredMinecartEvent?.Invoke(this);
            Despawn();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        collider.isTrigger = true;
    }

    public void SetKinematic()
    {
        rb.isKinematic = !rb.isKinematic;
    }
}
