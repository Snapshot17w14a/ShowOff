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

    public void DespawnAfter(float timeUntilDespawn)
    {
        despawnTime = Time.time + timeUntilDespawn;
        isDespawning = true;
    }

    public void Collect(Transform parent)
    {
        IsPickedUp = true;
        Rigidbody rb = GetComponent<Rigidbody>();
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

    private void Update()
    {
        if (isDespawning && Time.time >= despawnTime)
        {
            Despawn();
        }
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
            Rigidbody rb = GetComponent<Rigidbody>();
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
}
